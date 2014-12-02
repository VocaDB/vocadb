using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.UseCases {

	public class AlbumForArtistEditContract {

		public AlbumForArtistEditContract() { }

		public AlbumForArtistEditContract(ArtistForAlbum artistForAlbum, ContentLanguagePreference languagePreference) {

			AlbumId = artistForAlbum.Album.Id;
			AlbumName = artistForAlbum.Album.TranslatedName[languagePreference];
			AlbumAdditionalNames = string.Join(", ", artistForAlbum.Album.AllNames.Where(n => n != AlbumName));
			ArtistForAlbumId = artistForAlbum.Id;

		}

		public AlbumForArtistEditContract(AlbumContract albumContract) {

			AlbumId = albumContract.Id;
			AlbumName = albumContract.Name;
			AlbumAdditionalNames = albumContract.AdditionalNames;

		}

		public AlbumForArtistEditContract(string name) {

			AlbumName = name;

		}

		public string AlbumAdditionalNames { get; set; }

		public int AlbumId { get; set; }

		public string AlbumName { get; set; }

		public int ArtistForAlbumId { get; set; }

	}

}
