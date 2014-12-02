using System.Linq;
using NHibernate;
using VocaDb.Model;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	public class EventQueries : QueriesBase<IEventRepository, ReleaseEvent> {

		private readonly IEntryLinkFactory entryLinkFactory;

		private void Archive(IRepositoryContext<ReleaseEvent> ctx, ReleaseEvent releaseEvent, ReleaseEventDiff diff, EntryEditEvent reason) {

			var agentLoginData = ctx.OfType<User>().CreateAgentLoginData(permissionContext);
			var archived = releaseEvent.CreateArchivedVersion(diff, agentLoginData, reason);
			ctx.Save(archived);

		}

		private void UpdateAllReleaseEventNames(IRepositoryContext<ReleaseEvent> ctx, string old, string newName) {

			ParamIs.NotNullOrWhiteSpace(() => old);
			ParamIs.NotNullOrWhiteSpace(() => newName);

			old = old.Trim();
			newName = newName.Trim();

			if (old.Equals(newName))
				return;

			ctx.AuditLogger.AuditLog(string.Format("replacing release event name '{0}' with '{1}'", old, newName));

			var albums = ctx.OfType<Album>()
				.Query()
				.Where(a => a.OriginalRelease.EventName == old)
				.ToArray();

			foreach (var a in albums) {
				NHibernateUtil.Initialize(a.CoverPictureData);
				a.OriginalRelease.EventName = newName;
				ctx.Update(a);
			}

		}

		public EventQueries(IEventRepository eventRepository, IEntryLinkFactory entryLinkFactory, IUserPermissionContext permissionContext)
			: base(eventRepository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;

		}

		public ReleaseEventContract[] List(EventSortRule sortRule, bool includeSeries = false) {
			
			return repository.HandleQuery(ctx => ctx
				.Query()
				.Where(e => e.Date != null)
				.OrderBy(sortRule)
				.ToArray()
				.Select(e => new ReleaseEventContract(e, includeSeries))
				.ToArray());

		}

		/// <summary>
		/// Updates or creates release event.
		/// 
		/// Album release event names will be updated as well if the name changed.
		/// </summary>
		/// <param name="contract">Updated contract. Cannot be null.</param>
		/// <returns>Updated release event data. Cannot be null.</returns>
		public ReleaseEventContract Update(ReleaseEventDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyManageDatabase();

			return repository.HandleTransaction(session => {

				ReleaseEvent ev;

				if (contract.Id == 0) {

					if (contract.Series != null) {
						var series = session.OfType<ReleaseEventSeries>().Load(contract.Series.Id);
						ev = new ReleaseEvent(contract.Description, contract.Date, series, contract.SeriesNumber, contract.SeriesSuffix);
						series.Events.Add(ev);
					} else {
						ev = new ReleaseEvent(contract.Description, contract.Date, contract.Name);
					}

					session.Save(ev);

					Archive(session, ev, new ReleaseEventDiff(), EntryEditEvent.Created);

					session.AuditLogger.AuditLog("created " + ev);

				} else {

					ev = session.Load(contract.Id);
					var diff = new ReleaseEventDiff();

					if (ev.Date != contract.Date)
						diff.Date = true;

					if (ev.Description != contract.Description)
						diff.Description = true;

					if (ev.Name != contract.Name)
						diff.Name = true;

					if (ev.SeriesNumber != contract.SeriesNumber)
						diff.SeriesNumber = true;

					if (ev.SeriesSuffix != contract.SeriesSuffix)
						diff.SeriesSuffix = true;

					var oldName = ev.Name;

					ev.Date = contract.Date;
					ev.Description = contract.Description;
					ev.Name = contract.Name;
					ev.SeriesNumber = contract.SeriesNumber;
					ev.SeriesSuffix = contract.SeriesSuffix;
					ev.UpdateNameFromSeries();

					UpdateAllReleaseEventNames(session, oldName, ev.Name);

					session.Update(ev);

					Archive(session, ev, diff, EntryEditEvent.Updated);

					var logStr = string.Format("updated properties for {0} ({1})", entryLinkFactory.CreateEntryLink(ev), diff.ChangedFieldsString);
					session.AuditLogger.AuditLog(logStr);

				}

				return new ReleaseEventContract(ev);

			});

		}

	}

	public enum EventSortRule {
		
		None,

		Name,

		Date,

		SeriesName

	}

	public static class EventQueryableExtender {

		public static IQueryable<ReleaseEvent> OrderBy(this IQueryable<ReleaseEvent> query, EventSortRule sortRule) {

			switch (sortRule) {
				case EventSortRule.Date:
					return query.OrderByDescending(r => r.Date);
				case EventSortRule.Name:
					return query.OrderBy(r => r.Name);
				case EventSortRule.SeriesName:
					return query
						.OrderBy(r => r.Series.Name)
						.ThenBy(r => r.SeriesNumber);
			}

			return query;

		}

		public static IQueryable<ReleaseEvent> WhereHasName(this IQueryable<ReleaseEvent> query, string name) {
			
			if (string.IsNullOrEmpty(name))
				return query;

			return (name.Length < 3) ? query.Where(e => e.Name.StartsWith(name)) : query.Where(e => e.Name.Contains(name));

		} 

		public static IQueryable<ReleaseEvent> WhereHasSeries(this IQueryable<ReleaseEvent> query, int seriesId) {
			
			if (seriesId == 0)
				return query;

			return query.Where(e => e.Series.Id == seriesId);

		} 

	}

}