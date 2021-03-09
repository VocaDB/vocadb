#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.DataContracts.SongImport
{
	[DataContract]
	public class ImportedSongInListContract
	{
		public ImportedSongInListContract() { }

		public ImportedSongInListContract(PVService service, string pvId)
		{
			PVService = service;
			PVId = pvId;
		}

		[DataMember]
		public SongForApiContract MatchedSong { get; set; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public string PVId { get; init; }

		[DataMember]
		public PVService PVService { get; init; }

		[DataMember]
		public int SortIndex { get; init; }

		[DataMember]
		public string Url { get; init; }
	}
}
