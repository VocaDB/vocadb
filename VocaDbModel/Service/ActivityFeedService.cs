using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service {

	public class ActivityFeedService : ServiceBase {

		private readonly IEntryThumbPersister entryThumbPersister;
		private readonly IEntryImagePersisterOld entryImagePersisterOld;

		public ActivityFeedService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory,
			IEntryThumbPersister entryThumbPersister, IEntryImagePersisterOld entryImagePersisterOld) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {

			this.entryThumbPersister = entryThumbPersister;
			this.entryImagePersisterOld = entryImagePersisterOld;

		}

		public PartialFindResult<ActivityEntryContract> GetFollowedArtistActivity(int maxEntries, bool ssl) {

			if (!PermissionContext.IsLoggedIn)
				return new PartialFindResult<ActivityEntryContract>();

			return HandleQuery(session => {

				var userId = PermissionContext.LoggedUserId;

				var albumEntries = session.Query<AlbumActivityEntry>()
					.Where(a => !a.Entry.Deleted && a.EditEvent == EntryEditEvent.Created && a.Entry.AllArtists.Any(r => r.Artist.Users.Any(u => u.User.Id == userId)))
					.OrderByDescending(a => a.CreateDate)
					.Take(maxEntries)
					.ToArray();

				var songEntries = session.Query<SongActivityEntry>()
					.Where(a => !a.Entry.Deleted && a.EditEvent == EntryEditEvent.Created && a.Entry.AllArtists.Any(r => r.Artist.Users.Any(u => u.User.Id == userId)))
					.OrderByDescending(a => a.CreateDate)
					.Take(maxEntries)
					.ToArray();

				var contracts = albumEntries.Cast<ActivityEntry>()
					.Concat(songEntries)
					.OrderByDescending(a => a.CreateDate)
					.Take(maxEntries)
					.Select(e => new ActivityEntryContract(e, PermissionContext.LanguagePreference, entryThumbPersister, entryImagePersisterOld, ssl))
					.ToArray();

				return new PartialFindResult<ActivityEntryContract>(contracts, 0);

			});

		}


	}
}
