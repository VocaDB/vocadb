using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists
{

	public class ArtistWithPictureContract : ArtistContract
	{

		public ArtistWithPictureContract(Artist artist, ContentLanguagePreference languagePreference)
			: base(artist, languagePreference)
		{

			CoverPicture = (artist.Picture != null ? new PictureContract(artist.Picture, artist.PictureMime) : null);

		}

		public PictureContract CoverPicture { get; set; }

	}
}
