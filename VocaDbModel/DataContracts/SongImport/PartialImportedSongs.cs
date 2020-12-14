#nullable disable

using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.SongImport
{
	[DataContract]
	public class PartialImportedSongs
	{
		public PartialImportedSongs() { }

		public PartialImportedSongs(ImportedSongInListContract[] items, int totalCount, string nextPageToken)
		{
			Items = items;
			TotalCount = totalCount;
			NextPageToken = nextPageToken;
		}

		[DataMember]
		public ImportedSongInListContract[] Items { get; set; }

		[DataMember]
		public string NextPageToken { get; set; }

		[DataMember]
		public int TotalCount { get; set; }
	}
}
