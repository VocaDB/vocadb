using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Ranking {

	[DataContract]
	public class SongInRankingContract {

		public SongInRankingContract() {}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NicoId { get; set; }

		[DataMember]
		public int SongId { get; set; }

		[DataMember]
		public int SortIndex { get; set; }

		public string Url { get; set; }

	}
}
