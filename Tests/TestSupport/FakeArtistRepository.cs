using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeArtistRepository : FakeRepository<Artist>, IArtistRepository {

		public FakeArtistRepository() { }

		public FakeArtistRepository(params Artist[] artists)
			: base(artists) { }

	}
}
