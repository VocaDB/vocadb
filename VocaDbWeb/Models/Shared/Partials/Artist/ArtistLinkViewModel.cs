using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Web.Models.Shared.Partials.Artist {

	public class ArtistLinkViewModel {

		public ArtistLinkViewModel(ArtistContract artist, bool typeLabel = false, string name = null, bool releaseYear = false) {
			Artist = artist;
			TypeLabel = typeLabel;
			Name = name;
			ReleaseYear = releaseYear;
		}

		public ArtistContract Artist { get; set; }

		public bool TypeLabel { get; set; }

		public string Name { get; set; }

		public bool ReleaseYear { get; set; }

	}

}