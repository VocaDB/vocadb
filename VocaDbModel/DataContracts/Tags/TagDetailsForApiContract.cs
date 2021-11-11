using System;
using System.Collections.Generic;
using System.Linq;
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
	public sealed record TagStatsForApiContract
	{
		public int AlbumCount { get; init; }

		public int ArtistCount { get; init; }

		public AlbumForApiContract[] Albums { get; init; }

		public ArtistForApiContract[] Artists { get; init; }

		public int EventCount { get; init; }

		public int EventSeriesCount { get; init; }

		public ReleaseEventForApiContract[] Events { get; init; }

		public ReleaseEventSeriesForApiContract[] EventSeries { get; init; }

		public int FollowerCount { get; init; }

		public int SongListCount { get; init; }

		public SongListBaseContract[] SongLists { get; init; }

		public SongForApiContract[] Songs { get; init; }

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
					fields: SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl
				))
				.ToArray();
			SongCount = songCount;

			FollowerCount = followerCount;
		}
	}

	public sealed record TagDetailsForApiContract
	{
		/// <summary>
		/// Additional names - optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public string CategoryName { get; init; }

		public TagBaseContract[] Children { get; init; }

		public int CommentCount { get; init; }

		/// <summary>
		/// Date this entry was created.
		/// </summary>
		[DataMember]
		public DateTime CreateDate { get; init; }

		public bool Deleted { get; init; }

		public EnglishTranslatedStringContract Description { get; init; }

		[DataMember]
		public int Id { get; set; }

		public bool IsFollowing { get; init; }

		public CommentForApiContract[] LatestComments { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract? MainPicture { get; init; }

		public string[] MappedNicoTags { get; init; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public TagBaseContract? Parent { get; set; }

		public EntryTypeAndSubType RelatedEntryType { get; init; }

		public TagBaseContract[] RelatedTags { get; init; }

		public TagBaseContract[] Siblings { get; init; }

		public TagStatsForApiContract Stats { get; init; }

		[DataMember]
		public EntryStatus Status { get; init; }

		[DataMember]
		public int Targets { get; init; }

		public string Translations { get; init; }

		[DataMember]
		public string UrlSlug { get; init; }

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
				.Select(w => new WebLinkForApiContract(w, WebLinkOptionalFields.DescriptionOrUrl))
				.OrderBy(w => w.DescriptionOrUrl)
				.ToArray();
		}

		public int AllUsageCount => Stats.ArtistCount + Stats.AlbumCount + Stats.SongCount + Stats.EventCount + Stats.SongListCount;
	}
}
