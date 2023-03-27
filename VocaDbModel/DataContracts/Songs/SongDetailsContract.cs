#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.DataContracts.Songs;

[Obsolete]
[DataContract(Namespace = Schemas.VocaDb)]
public class SongDetailsContract
{
	public SongDetailsContract() { }

	public SongDetailsContract(
		Song song,
		ContentLanguagePreference languagePreference,
		SongListBaseContract[] pools,
		ISpecialTags specialTags,
		IEntryTypeTagRepository entryTypeTags,
		IUserPermissionContext userContext,
		IAggregatedEntryImageUrlFactory thumbPersister,
		Tag songTypeTag = null
	)
	{
		Song = new SongContract(song, languagePreference);

		AdditionalNames = song.Names.GetAdditionalNamesStringForLanguage(languagePreference);
		Albums = song.OnAlbums
			.OrderBy(a => a.OriginalReleaseDate.SortableDateTime)
			.Select(a => new AlbumContract(a, languagePreference, userContext))
			.ToArray();
		AlternateVersions = song.AlternateVersions.Select(s => new SongContract(s, languagePreference, getThumbUrl: false)).OrderBy(s => s.PublishDate).ToArray();
		Artists = song.Artists.Select(a => new ArtistForSongContract(a, languagePreference)).OrderBy(a => a.Name).ToArray();
		ArtistString = song.ArtistString[languagePreference];
		CanEditPersonalDescription = EntryPermissionManager.CanEditPersonalDescription(userContext, song);
		CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, song);
		CreateDate = song.CreateDate;
		Deleted = song.Deleted;
		LikeCount = song.UserFavorites.Count(f => f.Rating == SongVoteRating.Like);
		LyricsFromParents = song.GetLyricsFromParents(specialTags, entryTypeTags).Select(l => new LyricsForSongContract(l, false)).ToArray();
		Notes = song.Notes;
		OriginalVersion = song.OriginalVersion != null && !song.OriginalVersion.Deleted
			? new SongForApiContract(song.OriginalVersion, null, languagePreference, userContext, SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl)
			: null;

		PVs = song.PVs.Select(p => new PVContract(p)).ToArray();
		ReleaseEvent = song.ReleaseEvent != null && !song.ReleaseEvent.Deleted
			? new ReleaseEventForApiContract(
				song.ReleaseEvent,
				languagePreference,
				userContext,
				ReleaseEventOptionalFields.None,
				thumbPersister
			)
			: null;
		PersonalDescriptionText = song.PersonalDescriptionText;
		var author = song.PersonalDescriptionAuthor;
		PersonalDescriptionAuthor = author != null
			? new ArtistForApiContract(
				author,
				languagePreference,
				userContext,
				thumbPersister,
				ArtistOptionalFields.MainPicture
			)
			: null;
		SongTypeTag = songTypeTag != null ? new TagBaseContract(songTypeTag, languagePreference) : null;
		SubjectsFromParents = song.GetCharactersFromParents().Select(c => new ArtistForSongContract(c, languagePreference)).ToArray();
		Tags = song.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).OrderByDescending(t => t.Count).ToArray();
		TranslatedName = new TranslatedStringContract(song.TranslatedName);
		WebLinks = song.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		Pools = pools;

		MinMilliBpm = song.MinMilliBpm;
		MaxMilliBpm = song.MaxMilliBpm;
	}

	/// <summary>
	/// Album id of the album being browsed.
	/// Null if none.
	/// </summary>
	public AlbumContract Album { get; set; }

	[DataMember]
	public AlbumContract[] Albums { get; init; }

	[DataMember]
	public SongInAlbumContract AlbumSong { get; set; }

	[DataMember]
	public SongContract[] AlternateVersions { get; init; }

	[DataMember]
	public string AdditionalNames { get; init; }

	[DataMember]
	public ArtistForSongContract[] Artists { get; init; }

	[DataMember]
	public string ArtistString { get; init; }

	public bool CanEditPersonalDescription { get; init; }

	public bool CanRemoveTagUsages { get; init; }

	[DataMember]
	public int CommentCount { get; set; }

	[DataMember]
	public DateTime CreateDate { get; init; }

	[DataMember]
	public bool Deleted { get; init; }

	[DataMember]
	public int Hits { get; set; }

	[DataMember]
	public CommentForApiContract[] LatestComments { get; set; }

	[DataMember]
	public int LikeCount { get; init; }

	[DataMember]
	public int ListCount { get; set; }

	[DataMember]
	public LyricsForSongContract[] LyricsFromParents { get; init; }

	[DataMember]
	public SongContract MergedTo { get; set; }

	/// <summary>
	/// Next song on the album being browsed (identified by AlbumId).
	/// Can be null.
	/// </summary>
	[DataMember]
	public SongInAlbumContract NextSong { get; set; }

	[DataMember]
	public EnglishTranslatedString Notes { get; init; }

	[DataMember]
	public SongForApiContract OriginalVersion { get; init; }

	[DataMember]
	public string PersonalDescriptionText { get; init; }

	[DataMember]
	public ArtistForApiContract PersonalDescriptionAuthor { get; init; }

	[DataMember]
	public SongListBaseContract[] Pools { get; init; }

	public LyricsForSongContract PreferredLyrics { get; set; }

	/// <summary>
	/// Previous song on the album being browsed (identified by AlbumId).
	/// Can be null.
	/// </summary>
	[DataMember]
	public SongInAlbumContract PreviousSong { get; set; }

	[DataMember(Name = "pvs")]
	public PVContract[] PVs { get; init; }

	[DataMember]
	public ReleaseEventForApiContract ReleaseEvent { get; init; }

	[DataMember]
	public SongContract Song { get; init; }

	[DataMember]
	public TagBaseContract SongTypeTag { get; init; }

	[DataMember]
	public ArtistForSongContract[] SubjectsFromParents { get; init; }

	[DataMember]
	public SongForApiContract[] Suggestions { get; set; }

	[DataMember]
	public TagUsageForApiContract[] Tags { get; init; }

	[DataMember]
	public TranslatedStringContract TranslatedName { get; init; }

	[DataMember]
	public SongVoteRating UserRating { get; set; }

	[DataMember]
	public WebLinkContract[] WebLinks { get; init; }

	[DataMember]
	public int? MinMilliBpm { get; init; }

	[DataMember]
	public int? MaxMilliBpm { get; init; }
}
