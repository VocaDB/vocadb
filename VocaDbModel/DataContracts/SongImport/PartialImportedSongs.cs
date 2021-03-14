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
		public ImportedSongInListContract[] Items { get; init; }

		[DataMember]
		public string NextPageToken { get; init; }

		[DataMember]
		public int TotalCount { get; init; }
	}
}
