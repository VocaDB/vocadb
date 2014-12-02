using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.MikuDb {

	public class InspectedAlbum {

		public InspectedAlbum() { 
		}

		public InspectedAlbum(MikuDbAlbumContract importedAlbum)
			: this() {

			ImportedAlbum = importedAlbum;

		}

		public InspectedArtist[] Artists { get; set; }

		public AlbumContract[] ExistingAlbums { get; set; }

		public MikuDbAlbumContract ImportedAlbum { get; set; }

		public AlbumContract MergedAlbum { get; set; }

		public int? MergedAlbumId { get; set; }

		public bool MergeTracks { get; set; }

		public ContentLanguageSelection SelectedLanguage { get; set; }

		public InspectedTrack[] Tracks { get; set; }

	}

	public class InspectedArtist {

		public InspectedArtist() { }

		public InspectedArtist(string artistName) {
			Name = artistName;
		}

		public ArtistContract ExistingArtist { get; set; }

		public string Name { get; set; }

	}

	public class InspectedTrack {

		public InspectedTrack() { }

		public InspectedTrack(ImportedAlbumTrack importedTrack) {
			ImportedTrack = importedTrack;
		}

		public SongContract ExistingSong { get; set; }

		public ImportedAlbumTrack ImportedTrack { get; set; }

		public bool Selected { get; set; }

	}

}
