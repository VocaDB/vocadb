using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeSongRepository : FakeRepository<Song>, ISongRepository {

		public FakeSongRepository(params Song[] songs)
			: base(songs) { }

	}

}
