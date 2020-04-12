using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Artists {

	public class ArtistWithPictureContract : ArtistContract {

		public ArtistWithPictureContract(Artist artist, ContentLanguagePreference languagePreference, ImageSize requestedSize)
			: base(artist, languagePreference) {

			CoverPicture = (artist.Picture != null ? new PictureContract(artist.Picture, artist.PictureMime, requestedSize) : null);

		}

		public PictureContract CoverPicture { get; set; }

	}
}
