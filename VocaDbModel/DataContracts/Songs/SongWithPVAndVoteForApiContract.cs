using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract]
	public sealed class SongWithPVAndVoteForApiContract : SongForApiContract
	{
		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongVoteRating Vote { get; init; }

		public SongWithPVAndVoteForApiContract(
			Song song,
			SongVoteRating vote,
			ContentLanguagePreference languagePreference
		)
			: base(
				song,
				mergeRecord: null,
				languagePreference,
				fields: SongOptionalFields.AdditionalNames | SongOptionalFields.PVs | SongOptionalFields.ThumbUrl
			)
		{
			Vote = vote;
		}
	}
}
