using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.SongImport
{
	[DataContract]
	public class ImportedSongListContract
	{
		public ImportedSongListContract()
		{
			Description = string.Empty;
		}

		public ImportedSongListContract(string name, DateTime createDate, string description, PartialImportedSongs songs, int wvrId)
		{
			CreateDate = createDate;
			Description = description;
			Name = name;
			Songs = songs;
			WVRNumber = wvrId;
		}

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public PartialImportedSongs Songs { get; set; }

		[DataMember]
		public int WVRNumber { get; set; }
	}
}
