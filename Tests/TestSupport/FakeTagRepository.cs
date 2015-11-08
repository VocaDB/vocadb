using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Tests.TestSupport {

	public class FakeTagRepository : FakeRepository<Tag>, ITagRepository {

		protected override ListDatabaseContext<Tag> CreateContext() {
			return new TagListDatabaseContext(querySource);
		}

		public FakeTagRepository(params Tag[] tags)
			: base(tags) {}

	}

	public class TagListDatabaseContext : ListDatabaseContext<Tag> {

		protected override object GetId(Tag entity) {
			return entity.Name;
		}

		public TagListDatabaseContext(QuerySourceList querySource) 
			: base(querySource) {}

	}

}
