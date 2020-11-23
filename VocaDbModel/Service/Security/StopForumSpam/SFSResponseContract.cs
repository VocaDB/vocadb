using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.Service.Security.StopForumSpam
{

	[DataContract]
	public class SFSResponseContract
	{

		/// <summary>
		/// Confidence threshold when user is considered spammer.
		/// </summary>
		public const double confidenceTreshold = 75d;

		/// <summary>
		/// IP/email appears in SFS database.
		/// </summary>
		[DataMember]
		public bool Appears { get; set; }

		public SFSCheckResultType Conclusion
		{
			get
			{
				if (Appears && Confidence > confidenceTreshold)
					return SFSCheckResultType.Malicious;
				if (Appears)
					return SFSCheckResultType.Uncertain;
				return SFSCheckResultType.Harmless;
			}
		}

		/// <summary>
		/// Conficence percentage that user is a spammer.
		/// Range 0-100.
		/// </summary>
		[DataMember]
		public double Confidence { get; set; }

		/// <summary>
		/// Number of times IP/email has been recorded in SFS database.
		/// </summary>
		[DataMember]
		public int Frequency { get; set; }

		[DataMember]
		public string IP { get; set; }

		[DataMember]
		public DateTime LastSeen { get; set; }

	}

}