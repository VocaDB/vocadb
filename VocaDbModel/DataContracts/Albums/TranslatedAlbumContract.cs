using System.Linq;
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

			var names = album.Names.Select(n => new LocalizedStringWithId(n.Value, n.Language)).ToArray();
			Names = new NameManager<LocalizedStringWithId> {
				Names = names, 
				SortNames = new TranslatedString(album.Names.SortNames)
			};

		}

		public TranslatedStringWithDefault TranslatedArtistString { get; set; }

		public NameManager<LocalizedStringWithId> Names { get; set; }

		public override string ToString() {
			return string.Format("translated album '{0}' [{1}]", Name, Id);
		}

	}

}
