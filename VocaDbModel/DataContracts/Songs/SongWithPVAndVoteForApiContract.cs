using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs;

[DataContract]
public sealed class SongWithPVAndVoteForApiContract : SongForApiContract
{
	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public SongVoteRating Vote { get; init; }

	public SongWithPVAndVoteForApiContract(
		Song song,
		SongVoteRating vote,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext
	)
		: base(
			song,
			mergeRecord: null,
			languagePreference,
			permissionContext,
			fields: SongOptionalFields.AdditionalNames | SongOptionalFields.PVs | SongOptionalFields.MainPicture
		)
	{
		Vote = vote;
	}
}
