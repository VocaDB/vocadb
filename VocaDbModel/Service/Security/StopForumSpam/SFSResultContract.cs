#nullable disable

using System.Runtime.Serialization;

namespace VocaDb.Model.Service.Security.StopForumSpam
{
	[DataContract]
	public class SFSResultContract
	{
		[DataMember]
		public bool Success { get; set; }

		[DataMember]
		public SFSResponseContract IP { get; set; }
	}
}