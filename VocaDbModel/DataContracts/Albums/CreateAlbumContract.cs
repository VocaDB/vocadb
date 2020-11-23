using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums
{
	public class CreateAlbumContract
	{
		public ArtistContract[] Artists { get; set; }

		public DiscType DiscType { get; set; }

		public LocalizedStringContract[] Names { get; set; }
	}
}
