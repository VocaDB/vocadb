using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeAlbumRepository : FakeRepository<Album>, IAlbumRepository {

		public FakeAlbumRepository() { }

		public FakeAlbumRepository(params Album[] albums)
			: base(albums) { }

	}

}
