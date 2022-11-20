using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public sealed record TagStatsForApiContract
	{
		[DataMember]
		public int AlbumCount { get; init; }

		[DataMember]
		public int ArtistCount { get; init; }

		[DataMember]
		public AlbumForApiContract[] Albums { get; init; }

		[DataMember]
		public ArtistForApiContract[] Artists { get; init; }

		[DataMember]
		public int EventCount { get; init; }

		[DataMember]
		public int EventSeriesCount { get; init; }

		[DataMember]
		public ReleaseEventForApiContract[] Events { get; init; }

		[DataMember]
		public ReleaseEventSeriesForApiContract[] EventSeries { get; init; }

		[DataMember]
		public int FollowerCount { get; init; }

		[DataMember]
		public int SongListCount { get; init; }

		[DataMember]
		public SongListBaseContract[] SongLists { get; init; }

		[DataMember]
		public SongForApiContract[] Songs { get; init; }

		[DataMember]
		public int SongCount { get; init; }

		public TagStatsForApiContract(
			ContentLanguagePreference languagePreference,
			IAggregatedEntryImageUrlFactory thumbStore,
			IEnumerable<Artist> artists,
			int artistCount,
			IEnumerable<Album> albums,
			int albumCount,
			IEnumerable<SongList> songLists,
			int songListCount,
			IEnumerable<Song> songs,
			int songCount,
			IEnumerable<ReleaseEventSeries> eventSeries,
			int eventSeriesCount,
			IEnumerable<ReleaseEvent> events,
			int eventCount,
			int followerCount
		)
		{
			Albums = albums
				.Select(a => new AlbumForApiContract(
					album: a,
					languagePreference: languagePreference,
					thumbPersister: thumbStore,
					fields: AlbumOptionalFields.AdditionalNames | AlbumOptionalFields.MainPicture
				))
				.ToArray();
			AlbumCount = albumCount;

			Artists = artists
				.Select(a => new ArtistForApiContract(
					artist: a,
					languagePreference: languagePreference,
					thumbPersister: thumbStore,
					includedFields: ArtistOptionalFields.AdditionalNames | ArtistOptionalFields.MainPicture
				))
				.ToArray();
			ArtistCount = artistCount;

			EventSeries = eventSeries
				.Select(a => new ReleaseEventSeriesForApiContract(
					series: a,
					languagePreference: languagePreference,
					fields: ReleaseEventSeriesOptionalFields.AdditionalNames | ReleaseEventSeriesOptionalFields.MainPicture,
					thumbPersister: thumbStore
				))
				.ToArray();
			EventSeriesCount = eventSeriesCount;

			Events = events
				.Select(a => new ReleaseEventForApiContract(
					rel: a,
					languagePreference: languagePreference,
					fields: ReleaseEventOptionalFields.AdditionalNames | ReleaseEventOptionalFields.MainPicture | ReleaseEventOptionalFields.Venue,
					thumbPersister: thumbStore
				))
				.ToArray();
			EventCount = eventCount;

			SongLists = songLists
				.Select(a => new SongListBaseContract(songList: a))
				.ToArray();
			SongListCount = songListCount;

			Songs = songs
				.Select(a => new SongForApiContract(
					song: a,
					languagePreference: languagePreference,
					fields: SongOptionalFields.AdditionalNames | SongOptionalFields.MainPicture
				))
				.ToArray();
			SongCount = songCount;

			FollowerCount = followerCount;
		}
	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public sealed record TagDetailsForApiContract
	{
		/// <summary>
		/// Additional names - optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; init; }

		[DataMember]
		public int AllUsageCount { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public string CategoryName { get; init; }

		[DataMember]
		public TagBaseContract[] Children { get; init; }

		[DataMember]
		public int CommentCount { get; init; }

		/// <summary>
		/// Date this entry was created.
		/// </summary>
		[DataMember]
		public DateTime CreateDate { get; init; }

		[DataMember]
		public bool Deleted { get; init; }

		[DataMember]
		public EnglishTranslatedStringContract Description { get; init; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public bool IsFollowing { get; init; }

		[DataMember]
		public CommentForApiContract[] LatestComments { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract? MainPicture { get; init; }

		[DataMember]
		public string[] MappedNicoTags { get; init; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public TagBaseContract? Parent { get; set; }

		[DataMember]
		public EntryTypeAndSubType RelatedEntryType { get; init; }

		[DataMember]
		public TagBaseContract[] RelatedTags { get; init; }

		[DataMember]
		public TagBaseContract[] Siblings { get; init; }

		[DataMember]
		public TagStatsForApiContract Stats { get; init; }

		[DataMember]
		public EntryStatus Status { get; init; }

		[DataMember]
		public int Targets { get; init; }

		[DataMember]
		public string Translations { get; init; }

		[DataMember]
		public string UrlSlug { get; init; }

		[DataMember]
		public WebLinkForApiContract[] WebLinks { get; init; }

		public TagDetailsForApiContract(
			Tag tag,
			TagStatsForApiContract stats,
			ContentLanguagePreference languagePreference,
			int commentCount,
			CommentForApiContract[] latestComments,
			bool isFollowing,
			EntryTypeAndSubType relatedEntryType,
			IAggregatedEntryImageUrlFactory thumbPersister
		)
		{
			AdditionalNames = tag.Names.AdditionalNamesString;
			AllUsageCount = stats.ArtistCount + stats.AlbumCount + stats.SongCount + stats.EventCount + stats.SongListCount;
			CategoryName = tag.CategoryName;
			Children = tag.Children
				.Select(a => new TagBaseContract(a, languagePreference))
				.OrderBy(t => t.Name)
				.ToArray();
			CommentCount = commentCount;
			CreateDate = tag.CreateDate;
			Deleted = tag.Deleted;
			Description = new EnglishTranslatedStringContract(tag.Description);
			Id = tag.Id;
			IsFollowing = isFollowing;
			LatestComments = latestComments;
			MainPicture = tag.Thumb is not null ? new EntryThumbForApiContract(tag.Thumb, thumbPersister) : null;
			MappedNicoTags = tag.Mappings.Select(t => t.SourceTag).ToArray();
			Name = tag.TranslatedName[languagePreference];
			Parent = tag.Parent is not null ? new TagBaseContract(tag.Parent, languagePreference) : null;
			RelatedEntryType = relatedEntryType;
			RelatedTags = tag.RelatedTags
				.Where(t => !t.LinkedTag.Deleted)
				.Select(a => new TagBaseContract(a.LinkedTag, languagePreference, true))
				.OrderBy(t => t.Name)
				.ToArray();
			Siblings = tag.Siblings
				.Select(a => new TagBaseContract(a, languagePreference))
				.OrderBy(t => t.Name)
				.ToArray();
			Stats = stats;
			Status = tag.Status;
			Targets = (int)tag.Targets;
			Translations = tag.Names.GetTranslationsString(languagePreference);
			UrlSlug = tag.UrlSlug;
			WebLinks = tag.WebLinks.Links
				.OrderBy(w => w.DescriptionOrUrl)
				.Select(w => new WebLinkForApiContract(w, WebLinkOptionalFields.DescriptionOrUrl))
				.ToArray();
		}
	}
}
