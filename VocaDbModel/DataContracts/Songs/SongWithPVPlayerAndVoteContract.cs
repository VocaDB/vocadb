#nullable disable

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
		public string PlayerHtml { get; init; }

		[DataMember]
		public string PVId { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public PVService PVService { get; init; }

		[DataMember]
		public SongWithPVAndVoteContract Song { get; init; }
	}
}
