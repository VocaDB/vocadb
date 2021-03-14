#nullable disable

using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists
{
	public class CreateArtistContract
	{
		public ArtistType ArtistType { get; init; }

		public string Description { get; init; }

		public bool Draft { get; init; }

		public LocalizedStringContract[] Names { get; init; }

		public EntryPictureFileContract PictureData { get; set; }

		public WebLinkContract WebLink { get; init; }
	}
}
