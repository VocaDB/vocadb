using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeUserMessageRepository : FakeRepository<UserMessage>, IUserMessageRepository {

		public FakeUserMessageRepository() { }

		public FakeUserMessageRepository(params UserMessage[] messages)
			: base(messages) { }

	}

}
