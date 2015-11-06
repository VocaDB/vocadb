using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.Service.Security.StopForumSpam {

	[DataContract]
	public class SFSResponseContract {

		public const double confidenceTreshold = 75d;

		[DataMember]
		public bool Appears { get; set; }

		public SFSCheckResultType Conclusion {
			get {
				if (Appears && Confidence > confidenceTreshold)
					return SFSCheckResultType.Malicious;
				if (Appears)
					return SFSCheckResultType.Uncertain;
				return SFSCheckResultType.Harmless;
			}
		}

		[DataMember]
		public double Confidence { get; set; }

		[DataMember]
		public int Frequency { get; set; }

		[DataMember]
		public string IP { get; set; }

		[DataMember]
		public DateTime LastSeen { get; set; }

	}

}