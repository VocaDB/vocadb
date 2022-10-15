#nullable disable

using NHibernate;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Venues;

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

		[Obsolete]
		public ReleaseEventWithArchivedVersionsContract GetReleaseEventWithArchivedVersions(int id)
		{
			return HandleQuery(session =>
				new ReleaseEventWithArchivedVersionsContract(session.Load<ReleaseEvent>(id), LanguagePreference, _userIconFactory));
		}

		[Obsolete]
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
