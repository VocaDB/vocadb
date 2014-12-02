using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ExportedAlbumContract : ArchivedAlbumContract {

		public ExportedAlbumContract() { }

		public ExportedAlbumContract(Album album, AlbumDiff diff)
			: base(album, diff) {

			Version = album.Version;

		}

		[DataMember]
		public int Version { get; set; }

	}
}
