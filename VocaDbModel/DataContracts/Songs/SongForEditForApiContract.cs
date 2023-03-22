using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record SongForEditForApiContract
{
	/// <summary>
	/// ID of the first album's release event.
	/// Used for validation warnings.
	/// </summary>
	[DataMember]
	public int? AlbumEventId { get; init; }

	[DataMember]
	public DateTime? AlbumReleaseDate { get; init; }

	[DataMember]
	public ArtistForSongContract[] Artists { get; set; }

	[DataMember]
	public bool CanDelete { get; set; }

	[DataMember]
	public ContentLanguageSelection DefaultNameLanguage { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public bool Deleted { get; init; }

	/// <summary>
	/// Song is on one or more albums
	/// </summary>
	[DataMember]
	public bool HasAlbums { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember]
	public int LengthSeconds { get; init; }

	[DataMember]
	public LyricsForSongContract[] Lyrics { get; set; }

	[DataMember]
	public int? MaxMilliBpm { get; init; }

	[DataMember]
	public int? MinMilliBpm { get; init; }

	/// <summary>
	/// Display name (primary name in selected language, or default language).
	/// </summary>
	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public LocalizedStringWithIdContract[] Names { get; init; }

	[DataMember]
	public EnglishTranslatedStringContract Notes { get; init; }

	[DataMember]
	public SongForApiContract? OriginalVersion { get; init; }

	[DataMember]
	public DateTime? PublishDate { get; init; }

	[DataMember(Name = "pvs")]
	public PVContract[] PVs { get; set; }

	[DataMember]
	public ReleaseEventForApiContract? ReleaseEvent { get; set; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public SongType SongType { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public EntryStatus Status { get; init; }

	// Required here for validation
	[DataMember]
	public int[] Tags { get; init; }

	[DataMember]
	public string UpdateNotes { get; set; }

	[DataMember]
	public WebLinkForApiContract[] WebLinks { get; set; }

	public SongForEditForApiContract()
	{
		Artists = Array.Empty<ArtistForSongContract>();
		Lyrics = Array.Empty<LyricsForSongContract>();
		Name = string.Empty;
		Names = Array.Empty<LocalizedStringWithIdContract>();
		Notes = new EnglishTranslatedStringContract();
		PVs = Array.Empty<PVContract>();
		Tags = Array.Empty<int>();
		UpdateNotes = string.Empty;
		WebLinks = Array.Empty<WebLinkForApiContract>();
	}

	public SongForEditForApiContract(Song song, ContentLanguagePreference languagePreference, IUserPermissionContext permissionContext)
	{
		var firstAlbum = song.Albums
			.Where(a => a.Album.OriginalReleaseDate.IsFullDate)
			.OrderBy(a => a.Album.OriginalReleaseDate)
			.FirstOrDefault();

		AlbumEventId = firstAlbum?.Album.OriginalReleaseEvent?.Id;
		AlbumReleaseDate = song.FirstAlbumDate is not null
			? DateTime.SpecifyKind(song.FirstAlbumDate.Value, DateTimeKind.Utc)
			: null;
		Artists = song.Artists
			.Select(a => new ArtistForSongContract(a, languagePreference))
			.OrderBy(a => a.Name)
			.ToArray();
		CanDelete = EntryPermissionManager.CanDelete(permissionContext, song);
		DefaultNameLanguage = song.TranslatedName.DefaultLanguage;
		Deleted = song.Deleted;
		HasAlbums = song.Albums.Any();
		Id = song.Id;
		LengthSeconds = song.LengthSeconds;
		Lyrics = song.Lyrics
			.Select(l => new LyricsForSongContract(l))
			.ToArray();
		MaxMilliBpm = song.MaxMilliBpm;
		MinMilliBpm = song.MinMilliBpm;
		Name = song.Names.SortNames[languagePreference];
		Names = song.Names
			.Select(n => new LocalizedStringWithIdContract(n))
			.ToArray();
		Notes = new EnglishTranslatedStringContract(song.Notes);
		OriginalVersion = song.OriginalVersion is not null && !song.OriginalVersion.Deleted
			? new SongForApiContract(
				song: song.OriginalVersion,
				languagePreference: languagePreference,
				permissionContext,
				fields: SongOptionalFields.None
			)
			: null;
		PublishDate = song.PublishDate.DateTime;
		PVs = song.PVs
			.Select(p => new PVContract(p))
			.ToArray();
		ReleaseEvent = song.ReleaseEvent is not null
			? new ReleaseEventForApiContract(
				rel: song.ReleaseEvent,
				languagePreference: languagePreference,
				fields: ReleaseEventOptionalFields.None,
				thumbPersister: null
			)
			: null;
		SongType = song.SongType;
		Status = song.Status;
		Tags = song.Tags.Tags
			.Select(t => t.Id)
			.ToArray();
		UpdateNotes = string.Empty;
		WebLinks = song.WebLinks
			.Select(w => new WebLinkForApiContract(w))
			.OrderBy(w => w.DescriptionOrUrl)
			.ToArray();
	}
}
