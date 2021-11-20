#nullable disable

using NHibernate;
using System.Linq;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service
{
	public class ReleaseEventService : ServiceBase
	{
		private readonly IUserIconFactory _userIconFactory;

		public ReleaseEventService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, IUserIconFactory userIconFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory)
		{
			_userIconFactory = userIconFactory;
		}

		public ReleaseEventSeriesWithEventsContract[] GetReleaseEventsBySeries()
		{
			return HandleQuery(session =>
			{
				var allEvents = session.Query<ReleaseEvent>().Where(e => !e.Deleted).ToArray();
				var series = session.Query<ReleaseEventSeries>().Where(e => !e.Deleted).OrderByName(LanguagePreference).ToArray();

				var seriesContracts = series.Select(s =>
					new ReleaseEventSeriesWithEventsContract(s, allEvents.Where(e => s.Equals(e.Series)), PermissionContext.LanguagePreference));
				var ungrouped = allEvents.Where(e => e.Series == null).OrderBy(e => e.TranslatedName[LanguagePreference]);

				return seriesContracts.Append(new ReleaseEventSeriesWithEventsContract
				{
					Name = string.Empty,
					Events = ungrouped.Select(e => new ReleaseEventContract(e, LanguagePreference)).ToArray()
				}).ToArray();
			});
		}

		public ReleaseEventWithArchivedVersionsContract GetReleaseEventWithArchivedVersions(int id)
		{
			return HandleQuery(session =>
				new ReleaseEventWithArchivedVersionsContract(session.Load<ReleaseEvent>(id), LanguagePreference, _userIconFactory));
		}

		public EntryWithArchivedVersionsContract<ReleaseEventSeriesContract, ArchivedEventSeriesVersionContract> GetReleaseEventSeriesWithArchivedVersions(int id)
		{
			return HandleQuery(session =>
			{
				var series = session.Load<ReleaseEventSeries>(id);
				return EntryWithArchivedVersionsContract.Create(new ReleaseEventSeriesContract(series, LanguagePreference), series.ArchivedVersionsManager.Versions.Select(v => new ArchivedEventSeriesVersionContract(v, _userIconFactory)).ToArray());
			});
		}

		public ReleaseEventSeriesForEditContract GetReleaseEventSeriesForEdit(int id)
		{
			return HandleQuery(session => new ReleaseEventSeriesForEditContract(session.Load<ReleaseEventSeries>(id), LanguagePreference));
		}

		public VenueForEditContract GetVenueForEdit(int id)
		{
			return HandleQuery(session => new VenueForEditContract(session.Load<Venue>(id), LanguagePreference));
		}
	}
}
