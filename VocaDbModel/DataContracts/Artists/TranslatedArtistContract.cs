using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	/// <summary>
	/// Artist contract with names for all supported language options.
	/// </summary>
	public class TranslatedArtistContract : ArtistContract {

		public TranslatedArtistContract() {}

		public TranslatedArtistContract(Artist artist)
			: base(artist, ContentLanguagePreference.Default) {

			Names = new BasicNameManager(artist.Names);

		}

		public BasicNameManager Names { get; set; }

		public override string ToString() {
			return string.Format("translated artist '{0}' [{1}]", Name, Id);
		}

	}

}
