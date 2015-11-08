using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Tests.TestSupport {

	public class FakeEventRepository : FakeRepository<ReleaseEvent>, IEventRepository {

		public FakeEventRepository() {}
		public FakeEventRepository(params ReleaseEvent[] items) : base(items) {}

	}

}
