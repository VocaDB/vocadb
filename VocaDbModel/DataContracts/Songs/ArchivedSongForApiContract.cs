using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ArchivedSongForApiContract
{
	[DataMember]
	public required AlbumForSongRefContract[]? Albums { get; init; }

	[DataMember]
	public required ArchivedArtistForSongContract[]? Artists { get; init; }

	[DataMember]
	public required int Id { get; init; }

	[DataMember]
	public required int LengthSeconds { get; init; }

	[DataMember]
	public required LyricsForSongContract[]? Lyrics { get; init; }

	[DataMember]
	public required int? MaxMilliBpm { get; init; }

	[DataMember]
	public required int? MinMilliBpm { get; init; }

	[DataMember]
	public required LocalizedStringContract[]? Names { get; init; }

	[DataMember]
	public required string? NicoId { get; init; }

	[DataMember]
	public required string Notes { get; init; }

	[DataMember]
	public required string NotesEng { get; init; }

	[DataMember]
	public required ObjectRefContract? OriginalVersion { get; init; }

	[DataMember]
	public required DateTime? PublishDate { get; init; }

	[DataMember]
	[JsonProperty("pvs")]
	public required ArchivedPVContract[]? PVs { get; init; }

	[DataMember]
	public required ObjectRefContract? ReleaseEvent { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public required SongType SongType { get; init; }

	[DataMember]
	public required ArchivedTranslatedStringContract TranslatedName { get; init; }

	[DataMember]
	public required ArchivedWebLinkContract[]? WebLinks { get; init; }

	public static ArchivedSongForApiContract Create(ArchivedSongContract contract, IUserPermissionContext permissionContext)
	{
		return new()
		{
			Albums = contract.Albums,
			Artists = contract.Artists,
			Id = contract.Id,
			LengthSeconds = contract.LengthSeconds,
			Lyrics = permissionContext.HasPermission(PermissionToken.ViewLyrics)
				? contract.Lyrics
				: null,
			MaxMilliBpm = contract.MaxMilliBpm,
			MinMilliBpm = contract.MinMilliBpm,
			Names = contract.Names,
			NicoId = contract.NicoId,
			Notes = contract.Notes,
			NotesEng = contract.NotesEng,
			OriginalVersion = contract.OriginalVersion,
			PublishDate = contract.PublishDate,
			PVs = permissionContext.HasPermission(PermissionToken.ViewOtherPVs)
				? contract.PVs
				: contract.PVs?.Where(pv => pv.PVType == PVType.Original).ToArray(),
			ReleaseEvent = contract.ReleaseEvent,
			SongType = contract.SongType,
			TranslatedName = contract.TranslatedName,
			WebLinks = contract.WebLinks,
		};
	}
}
