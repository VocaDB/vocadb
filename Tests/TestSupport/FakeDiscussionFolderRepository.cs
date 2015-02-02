using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeDiscussionFolderRepository : FakeRepository<DiscussionFolder>, IDiscussionFolderRepository {
		public FakeDiscussionFolderRepository(params DiscussionFolder[] items) : base(items) {}
		public FakeDiscussionFolderRepository() {}
		public FakeDiscussionFolderRepository(QuerySourceList querySource) : base(querySource) {}
	}

}
