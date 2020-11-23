using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.TestSupport
{
	public class FakeUserMessageRepository : FakeRepository<UserMessage>, IUserMessageRepository
	{
		public FakeUserMessageRepository() { }

		public FakeUserMessageRepository(params UserMessage[] messages)
			: base(messages) { }
	}
}
