using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Tests.TestSupport
{
	public class FakeFollowedArtistNotifier : IFollowedArtistNotifier
	{
		public User[] SendNotifications(IDatabaseContext ctx, IEntryWithNames entry, IEnumerable<Artist> artists, IUser creator)
		{
			return new User[0];
		}

		public Task<IReadOnlyCollection<User>> SendNotificationsAsync(IDatabaseContext ctx, IEntryWithNames entry, IEnumerable<Artist> artists, IUser creator)
		{
			return Task.FromResult((IReadOnlyCollection<User>)new List<User>());
		}
	}
}
