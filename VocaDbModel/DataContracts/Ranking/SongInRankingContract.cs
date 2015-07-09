using System.Runtime.Serialization;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.DataContracts.Ranking {

	[DataContract]
	public class SongInRankingContract {

		public SongInRankingContract() {}

		public SongInRankingContract(PVService service, string pvId) {
			PVService = service;
			PVId = pvId;
		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PVId { get; set; }

		[DataMember]
		public PVService PVService { get; set; }

		[DataMember]
		public int SongId { get; set; }

		[DataMember]
		public int SortIndex { get; set; }

		public string Url { get; set; }

	}
}
