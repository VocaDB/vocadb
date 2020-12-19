#nullable disable

using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service
{
	public class ActivityFeedService : ServiceBase
	{
		private readonly IUserIconFactory _userIconFactory;
		private readonly EntryForApiContractFactory _entryForApiContractFactory;

		public ActivityFeedService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory,
			IUserIconFactory userIconFactory, EntryForApiContractFactory entryForApiContractFactory)
			: base(sessionFactory, permissionContext, entryLinkFactory)
		{
			_userIconFactory = userIconFactory;
			_entryForApiContractFactory = entryForApiContractFactory;
		}

		public PartialFindResult<ActivityEntryForApiContract> GetFollowedArtistActivity(int maxEntries)
		{
			if (!PermissionContext.IsLoggedIn)
				return new PartialFindResult<ActivityEntryForApiContract>();

			return HandleQuery(session =>
			{
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
					.Select(e => new ActivityEntryForApiContract(e, _entryForApiContractFactory.Create(e.EntryBase, EntryOptionalFields.AdditionalNames | EntryOptionalFields.MainPicture,
						LanguagePreference), _userIconFactory,
					PermissionContext, ActivityEntryOptionalFields.None))
					.ToArray();

				return new PartialFindResult<ActivityEntryForApiContract>(contracts, 0);
			});
		}
	}
}
