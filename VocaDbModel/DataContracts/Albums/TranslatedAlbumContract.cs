using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums {

	/// <summary>
	/// Album contract with names for all supported language options.
	/// </summary>
	public class TranslatedAlbumContract : AlbumContract {

		public TranslatedAlbumContract() {}

		public TranslatedAlbumContract(Album album)
			: base(album, ContentLanguagePreference.Default) {

			TranslatedArtistString = TranslatedStringWithDefault.Create(n => album.ArtistString[n]);

			Names = new BasicNameManager(album.Names);

		}

		public TranslatedStringWithDefault TranslatedArtistString { get; set; }

		public BasicNameManager Names { get; set; }

		public override string ToString() {
			return string.Format("translated album '{0}' [{1}]", Name, Id);
		}

	}

}
