using VocaDb.Model.DataContracts.MikuDb;

namespace VocaDb.Model.DataContracts.Albums {

	public class ParseAlbumFileResultContract {

		public ParseAlbumFileResultContract() { }

		public ParseAlbumFileResultContract(AlbumContract album) {
			Album = album;
		}

		public AlbumContract Album { get; set; }

		public ImportedAlbumDataContract Imported { get; set; }

	}

}
