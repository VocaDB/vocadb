using System.Linq;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeTagRepository : FakeRepository<Tag>, ITagRepository {

		protected override ListRepositoryContext<Tag> CreateContext() {
			return new TagListRepositoryContext(querySource);
		}

		public FakeTagRepository(params Tag[] tags)
			: base(tags) {}

	}

	public class TagListRepositoryContext : ListRepositoryContext<Tag> {

		protected override object GetId(Tag entity) {
			return entity.Name;
		}

		public TagListRepositoryContext(QuerySourceList querySource) 
			: base(querySource) {}

	}

}
