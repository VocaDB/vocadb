using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Tests.TestSupport
{
	public class FakeEventRepository : FakeRepository<ReleaseEvent>, IEventRepository
	{
		public FakeEventRepository() { }
		public FakeEventRepository(params ReleaseEvent[] items) : base(items) { }
	}
}
