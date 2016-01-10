using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Tests.TestSupport {

	public class FakeSongRepository : FakeRepository<Song>, ISongRepository {

		public FakeSongRepository(params Song[] songs)
			: base(songs) { }

	}

}
