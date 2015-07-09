using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.DataContracts.SongImport {

	[DataContract]
	public class ImportedSongInListContract {

		public ImportedSongInListContract() {}

		public ImportedSongInListContract(PVService service, string pvId) {
			PVService = service;
			PVId = pvId;
		}

		[DataMember]
		public SongContract MatchedSong { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PVId { get; set; }

		[DataMember]
		public PVService PVService { get; set; }

		[DataMember]
		public int SortIndex { get; set; }

		[DataMember]
		public string Url { get; set; }

	}
}
