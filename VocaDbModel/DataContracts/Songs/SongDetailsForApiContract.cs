using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Globalization;
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

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public sealed record SongDetailsForApiContract
	{
		[DataMember]
		public string AdditionalNames { get; init; }

		/// <summary>
		/// Album id of the album being browsed.
		/// Null if none.
		/// </summary>
		[DataMember]
		public AlbumForApiContract? Album { get; set; }

		[DataMember]
		public AlbumForApiContract[] Albums { get; init; }

		[DataMember]
		public SongInAlbumForApiContract? AlbumSong { get; set; }

		[DataMember]
		public SongForApiContract[] AlternateVersions { get; init; }

		[DataMember]
		public ArtistForSongContract[] Artists { get; init; }

		[DataMember]
		public string? ArtistString { get; init; }

		[DataMember]
		public bool CanEditPersonalDescription { get; init; }

		[DataMember]
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
		public CommentForApiContract[] LatestComments { get; set; } = default!;

		[DataMember]
		public int LikeCount { get; init; }

		[DataMember]
		public int ListCount { get; set; }

		[DataMember]
		public LyricsForSongContract[] LyricsFromParents { get; init; }

		[DataMember]
		public int? MaxMilliBpm { get; init; }

		[DataMember]
		public SongForApiContract? MergedTo { get; set; }

		[DataMember]
		public int? MinMilliBpm { get; init; }

		/// <summary>
		/// Next song on the album being browsed (identified by AlbumId).
		/// Can be null.
		/// </summary>
		[DataMember]
		public SongInAlbumForApiContract? NextSong { get; set; }

		[DataMember]
		public EnglishTranslatedStringContract Notes { get; init; }

		[DataMember]
		public SongForApiContract? OriginalVersion { get; init; }

		[DataMember]
		public ArtistForApiContract? PersonalDescriptionAuthor { get; init; }

		[DataMember]
		public string? PersonalDescriptionText { get; init; }

		[DataMember]
		public SongListBaseContract[] Pools { get; init; }

		[DataMember]
		public LyricsForSongContract? PreferredLyrics { get; set; }

		/// <summary>
		/// Previous song on the album being browsed (identified by AlbumId).
		/// Can be null.
		/// </summary>
		[DataMember]
		public SongInAlbumForApiContract? PreviousSong { get; set; }

		[DataMember(Name = "pvs")]
		public PVContract[] PVs { get; init; }

		[DataMember]
		public ReleaseEventForApiContract? ReleaseEvent { get; init; }

		[DataMember]
		public SongForApiContract Song { get; init; }

		[DataMember]
		public TagBaseContract? SongTypeTag { get; init; }

		[DataMember]
		public ArtistForSongContract[] SubjectsFromParents { get; init; }

		[DataMember]
		public SongForApiContract[] Suggestions { get; set; } = default!;

		[DataMember]
		public TagUsageForApiContract[] Tags { get; init; }

		[DataMember]
		public SongVoteRating UserRating { get; set; }

		[DataMember]
		public WebLinkForApiContract[] WebLinks { get; init; }

		public SongDetailsForApiContract(
			Song song,
			ContentLanguagePreference languagePreference,
			SongListBaseContract[] pools,
			ISpecialTags? specialTags,
			IEntryTypeTagRepository? entryTypeTags,
			IUserPermissionContext userContext,
			IAggregatedEntryImageUrlFactory thumbPersister,
			Tag? songTypeTag = null
		)
		{
			Song = new SongForApiContract(song: song, languagePreference: languagePreference, fields: SongOptionalFields.None);
			AdditionalNames = song.Names.GetAdditionalNamesStringForLanguage(languagePreference);

			Albums = song.OnAlbums
				.OrderBy(a => a.OriginalReleaseDate.SortableDateTime)
				.Select(a => new AlbumForApiContract(
					album: a,
					languagePreference: languagePreference,
					thumbPersister: thumbPersister,
					fields: AlbumOptionalFields.None
				))
				.ToArray();

			AlternateVersions = song.AlternateVersions
				.Select(s => new SongForApiContract(
					song: s,
					languagePreference: languagePreference,
					fields: SongOptionalFields.None
				))
				.OrderBy(s => s.PublishDate)
				.ToArray();

			Artists = song.Artists
				.Select(a => new ArtistForSongContract(artistForSong: a, languagePreference: languagePreference))
				.OrderBy(a => a.Name)
				.ToArray();

			ArtistString = song.ArtistString[languagePreference];
			CanEditPersonalDescription = EntryPermissionManager.CanEditPersonalDescription(userContext, song);
			CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, song);
			CreateDate = song.CreateDate;
			Deleted = song.Deleted;
			LikeCount = song.UserFavorites.Count(f => f.Rating == SongVoteRating.Like);

			LyricsFromParents = song.GetLyricsFromParents(specialTags, entryTypeTags)
				.Select(l => new LyricsForSongContract(l, false))
				.ToArray();

			MaxMilliBpm = song.MaxMilliBpm;
			MinMilliBpm = song.MinMilliBpm;
			Notes = new EnglishTranslatedStringContract(song.Notes);

			OriginalVersion = song.OriginalVersion is not null && !song.OriginalVersion.Deleted
				? new SongForApiContract(
					song: song.OriginalVersion,
					mergeRecord: null,
					languagePreference: languagePreference,
					fields: SongOptionalFields.AdditionalNames | SongOptionalFields.MainPicture
				)
				: null;

			PersonalDescriptionAuthor = song.PersonalDescriptionAuthor is { } author
				? new ArtistForApiContract(
					artist: author,
					languagePreference: languagePreference,
					thumbPersister: thumbPersister,
					includedFields: ArtistOptionalFields.MainPicture
				)
				: null;

			PersonalDescriptionText = song.PersonalDescriptionText;
			Pools = pools;
			PVs = song.PVs.Select(p => new PVContract(pv: p)).ToArray();

			ReleaseEvent = song.ReleaseEvent is not null && !song.ReleaseEvent.Deleted
				? new ReleaseEventForApiContract(
					rel: song.ReleaseEvent,
					languagePreference: languagePreference,
					fields: ReleaseEventOptionalFields.None,
					thumbPersister: thumbPersister
				)
				: null;

			SongTypeTag = songTypeTag != null
				? new TagBaseContract(tag: songTypeTag, languagePreference: languagePreference)
				: null;

			SubjectsFromParents = song.GetCharactersFromParents()
				.Select(c => new ArtistForSongContract(c, languagePreference))
				.ToArray();

			Tags = song.Tags.ActiveUsages
				.Select(u => new TagUsageForApiContract(tagUsage: u, languagePreference: languagePreference))
				.OrderByDescending(t => t.Count)
				.ToArray();

			WebLinks = song.WebLinks
				.OrderBy(w => w.DescriptionOrUrl)
				.Select(w => new WebLinkForApiContract(w))
				.ToArray();
		}
	}
}
