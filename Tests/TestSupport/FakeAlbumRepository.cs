using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Tests.TestSupport {

	public class FakeAlbumRepository : FakeRepository<Album>, IAlbumRepository {

		public FakeAlbumRepository() { }

		public FakeAlbumRepository(params Album[] albums)
			: base(albums) { }

	}

}
