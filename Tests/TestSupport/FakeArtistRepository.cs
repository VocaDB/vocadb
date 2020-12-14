#nullable disable

using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Tests.TestSupport
{
	public class FakeArtistRepository : FakeRepository<Artist>, IArtistRepository
	{
		public FakeArtistRepository() { }

		public FakeArtistRepository(params Artist[] artists)
			: base(artists) { }
	}
}
