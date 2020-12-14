#nullable disable

using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.MikuDb
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ImportedAlbumDataContract
	{
		public ImportedAlbumDataContract()
		{
			ArtistNames = new string[] { };
			Tracks = new ImportedAlbumTrack[] { };
			VocalistNames = new string[] { };
		}

		[DataMember]
		public string[] ArtistNames { get; set; }

		[DataMember]
		public string CircleName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int? ReleaseYear { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public ImportedAlbumTrack[] Tracks { get; set; }

		[DataMember]
		public string[] VocalistNames { get; set; }
	}
}
