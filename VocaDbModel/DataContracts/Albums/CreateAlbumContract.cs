#nullable disable

using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums
{
	public class CreateAlbumContract
	{
		public ArtistContract[] Artists { get; init; }

		public DiscType DiscType { get; init; }

		public LocalizedStringContract[] Names { get; init; }
	}
}
