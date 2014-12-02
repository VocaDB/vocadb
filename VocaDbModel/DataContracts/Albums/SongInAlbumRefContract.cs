using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongInAlbumRefContract : ObjectRefContract {

		public SongInAlbumRefContract() {
			DiscNumber = 1;
		}

		public SongInAlbumRefContract(SongInAlbum songInAlbum) {

			DiscNumber = songInAlbum.DiscNumber;
			TrackNumber = songInAlbum.TrackNumber;

			var song = songInAlbum.Song;
			if (song != null) {
				Id = song.Id;
				NameHint = song.DefaultName;
			} else {
				NameHint = songInAlbum.Name;
			}

		}

		[DataMember]
		public int DiscNumber { get; set; }

		[DataMember]
		public int TrackNumber { get; set; }

	}

}
