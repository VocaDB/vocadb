using System;
using System.Linq;
using NHibernate;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Database.Queries {

	public class EventQueries : QueriesBase<IEventRepository, ReleaseEvent> {

		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEntryThumbPersister imagePersister;

		private ArchivedReleaseEventVersion Archive(IDatabaseContext<ReleaseEvent> ctx, ReleaseEvent releaseEvent, ReleaseEventDiff diff, EntryEditEvent reason) {

			var agentLoginData = ctx.OfType<User>().CreateAgentLoginData(permissionContext);
			var archived = releaseEvent.CreateArchivedVersion(diff, agentLoginData, reason);
			ctx.Save(archived);
			return archived;

		}

		private void CheckDuplicateName(IDatabaseContext<ReleaseEvent> ctx, ReleaseEvent ev) {

			var hasDuplicate = ctx.Query().Any(e => e.Id != ev.Id && e.Name == ev.Name);

			if (hasDuplicate) {
				throw new DuplicateEventNameException(ev.Name);
			}

		}

		private void UpdateAllReleaseEventNames(IDatabaseContext<ReleaseEvent> ctx, string old, string newName) {

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

		public EventQueries(IEventRepository eventRepository, IEntryLinkFactory entryLinkFactory, IUserPermissionContext permissionContext,
			IEntryThumbPersister imagePersister)
			: base(eventRepository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;

		}

		public void Delete(int eventId) {
			
			repository.HandleTransaction(ctx => {
				
				var entry = ctx.Load(eventId);

				ctx.AuditLogger.SysLog(string.Format("deleting {0}", entry));

				foreach (var song in entry.AllSongs) {
					song.ReleaseEvent = null;
				}

				var ctxActivity = ctx.OfType<ReleaseEventActivityEntry>();
				var activityEntries = ctxActivity.Query().Where(a => a.Entry.Id == eventId).ToArray();

				foreach (var activityEntry in activityEntries)
					ctxActivity.Delete(activityEntry);

				ctx.Delete(entry);

				ctx.AuditLogger.AuditLog(string.Format("deleted {0}", entry));

			});

		}

		public PartialFindResult<TResult> Find<TResult>(Func<ReleaseEvent, TResult> fac, SearchTextQuery textQuery,
			int seriesId,
			DateTime? afterDate,
			DateTime? beforeDate,
			int start,
			int maxResults,
			bool getTotalCount,
			EventSortRule sort) {

			return HandleQuery(ctx => {

				var q = ctx.Query()
					.WhereHasName(textQuery)
					.WhereHasSeries(seriesId)
					.WhereDateIsBetween(afterDate, beforeDate);

				var entries = q
					.OrderBy(sort)
					.Skip(start)
					.Take(maxResults)
					.ToArray()
					.Select(fac)
					.ToArray();

				var count = 0;

				if (getTotalCount) {

					count = q.Count();

				}

				return new PartialFindResult<TResult>(entries, count);

			});

		}

		public PartialFindResult<TResult> FindSeries<TResult>(Func<ReleaseEventSeries, TResult> fac, 
			SearchTextQuery textQuery, PagingProperties paging) {

			return HandleQuery(ctx => {

				var q = ctx.Query<ReleaseEventSeries>()
					.WhereHasName(textQuery)
					.Paged(paging);

				var entries = q
					.OrderBy(s => s.Name)
					.ToArray()
					.Select(fac)
					.ToArray();

				var count = paging.GetTotalCount ? q.Count() : 0;

				return PartialFindResult.Create(entries, count);

			});

		}

		public ReleaseEventContract[] List(EventSortRule sortRule, bool includeSeries = false) {
			
			return repository.HandleQuery(ctx => ctx
				.Query()
				.Where(e => e.Date.DateTime != null)
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

					var diff = new ReleaseEventDiff();

					if (!contract.Series.IsNullOrDefault()) {
						var series = session.OfType<ReleaseEventSeries>().Load(contract.Series.Id);
						ev = new ReleaseEvent(contract.Description, contract.Date, series, contract.SeriesNumber, contract.SeriesSuffix, 
							contract.Name, contract.CustomName);
						series.Events.Add(ev);
					} else {
						ev = new ReleaseEvent(contract.Description, contract.Date, contract.Name);
					}

					var weblinksDiff = WebLink.Sync(ev.WebLinks, contract.WebLinks, ev);

					if (weblinksDiff.Changed) {
						diff.WebLinks.Set();
					}

					CheckDuplicateName(session, ev);

					session.Save(ev);

					var archived = Archive(session, ev, diff, EntryEditEvent.Created);
					AddEntryEditedEntry(session.OfType<ActivityEntry>(), archived);

					session.AuditLogger.AuditLog(string.Format("created {0}", entryLinkFactory.CreateEntryLink(ev)));

				} else {

					ev = session.Load(contract.Id);
					var diff = new ReleaseEventDiff(DoSnapshot(ev, session));

					if (!ev.Date.Equals(contract.Date))
						diff.Date.Set();

					if (ev.Description != contract.Description)
						diff.Description.Set();

					if (ev.Name != contract.Name && (contract.Series == null || contract.CustomName))
						diff.Name.Set();

					if (!ev.Series.NullSafeIdEquals(contract.Series)) {
						diff.Series.Set();
					}

					if (ev.SeriesNumber != contract.SeriesNumber)
						diff.SeriesNumber.Set();

					if (ev.SeriesSuffix != contract.SeriesSuffix)
						diff.SeriesSuffix.Set();

					var oldName = ev.Name;

					ev.Series = session.OfType<ReleaseEventSeries>().NullSafeLoad(contract.Series);
					ev.CustomName = contract.CustomName;
					ev.Date = contract.Date;
					ev.Description = contract.Description;
					ev.Name = contract.Name;
					ev.SeriesNumber = contract.SeriesNumber;
					ev.SeriesSuffix = contract.SeriesSuffix;
					ev.UpdateNameFromSeries();

					var weblinksDiff = WebLink.Sync(ev.WebLinks, contract.WebLinks, ev);

					if (weblinksDiff.Changed) {
						diff.WebLinks.Set();
						session.OfType<ReleaseEventWebLink>().Sync(weblinksDiff);
					}

					CheckDuplicateName(session, ev);

					UpdateAllReleaseEventNames(session, oldName, ev.Name);

					session.Update(ev);

					var archived = Archive(session, ev, diff, EntryEditEvent.Updated);
					AddEntryEditedEntry(session.OfType<ActivityEntry>(), archived);

					var logStr = string.Format("updated properties for {0} ({1})", entryLinkFactory.CreateEntryLink(ev), diff.ChangedFieldsString);
					session.AuditLogger.AuditLog(logStr);

				}

				return new ReleaseEventContract(ev);

			});

		}

		private void SaveImage(ReleaseEventSeries series, EntryPictureFileContract pictureData) {

			if (pictureData != null) {

				var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);
				series.PictureMime = parsed.Mime;

				pictureData.Id = series.Id;
				pictureData.EntryType = EntryType.ReleaseEventSeries;
				var thumbGenerator = new ImageThumbGenerator(imagePersister);
				thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ReleaseEventSeries.ImageSizes, originalSize: Constants.RestrictedImageOriginalSize);

			}

		}

		public int UpdateSeries(ReleaseEventSeriesForEditContract contract, EntryPictureFileContract pictureData) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.ManageEventSeries);

			return HandleTransaction(session => {

				ReleaseEventSeries series;

				if (contract.Id == 0) {

					series = new ReleaseEventSeries(contract.Name, contract.Description, contract.Aliases);
					session.Save(series);

					var weblinksDiff = WebLink.Sync(series.WebLinks, contract.WebLinks, series);

					if (weblinksDiff.Changed) {
						session.OfType<ReleaseEventWebLink>().Sync(weblinksDiff);
					}

					if (pictureData != null) {
						SaveImage(series, pictureData);
						session.Update(series);
					}

					session.Update(series);

					AuditLog(string.Format("created {0}", entryLinkFactory.CreateEntryLink(series)), session);

				} else {

					series = session.Load<ReleaseEventSeries>(contract.Id);

					series.Name = contract.Name;
					series.Description = contract.Description;
					series.UpdateAliases(contract.Aliases);
					SaveImage(series, pictureData);

					var weblinksDiff = WebLink.Sync(series.WebLinks, contract.WebLinks, series);

					if (weblinksDiff.Changed) {
						session.OfType<ReleaseEventWebLink>().Sync(weblinksDiff);
					}

					session.Update(series);

					AuditLog(string.Format("updated {0}", entryLinkFactory.CreateEntryLink(series)), session);

				}

				return series.Id;

			});

		}

	}

}