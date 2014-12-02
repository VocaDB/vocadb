using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Ranking {

	[DataContract]
	public class RankingContract {

		public RankingContract() {
			Description = string.Empty;
			NicoId = string.Empty;
		}

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string NicoId { get; set; }

		[DataMember]
		public SongInRankingContract[] Songs { get; set; }

		[DataMember]
		public int WVRId { get; set; }

	}

}
