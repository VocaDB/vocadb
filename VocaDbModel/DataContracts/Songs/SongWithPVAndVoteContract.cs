#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs;

[Obsolete]
[DataContract]
public class SongWithPVAndVoteContract : SongContract
{
	public SongWithPVAndVoteContract(
		Song song,
		SongVoteRating vote,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		bool includePVs = true
	)
		: base(song, languagePreference)
	{
		if (includePVs)
		{
			PVs = (permissionContext.HasPermission(PermissionToken.ViewOtherPVs) ? song.PVs : song.OriginalPVs)
				.Select(p => new PVContract(p))
				.ToArray();
		}
		Vote = vote;
	}

	[DataMember(Name = "pvs")]
	public PVContract[] PVs { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public SongVoteRating Vote { get; init; }
}
