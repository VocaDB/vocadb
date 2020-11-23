using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.TestSupport
{
	public class FakeUserRepository : FakeRepository<User>, IUserRepository
	{
		public FakeUserRepository() { }

		public FakeUserRepository(params User[] users)
			: base(users) { }
	}
}
