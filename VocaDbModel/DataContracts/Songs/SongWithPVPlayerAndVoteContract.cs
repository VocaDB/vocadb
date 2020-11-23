using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongWithPVPlayerAndVoteContract : SongContract
	{
		[DataMember]
		public string PlayerHtml { get; set; }

		[DataMember]
		public string PVId { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public PVService PVService { get; set; }

		[DataMember]
		public SongWithPVAndVoteContract Song { get; set; }
	}
}
