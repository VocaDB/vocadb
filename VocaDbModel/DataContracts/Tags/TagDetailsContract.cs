using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagDetailsContract : TagContract, IEntryWithStatus {

		string IEntryBase.DefaultName => Name;

		EntryType IEntryBase.EntryType => EntryType.Tag;

		public TagDetailsContract() { }

		public TagDetailsContract(Tag tag, 
			IEnumerable<Artist> artists, int artistCount, IEnumerable<Album> albums, int albumCount,
			IEnumerable<SongList> songLists, int songListCount,
			IEnumerable<Song> songs, int songCount,
			IEnumerable<ReleaseEventSeries> eventSeries, int eventSeriesCount,
			IEnumerable<ReleaseEvent> events, int eventCount, 
			ContentLanguagePreference languagePreference,
			IEntryThumbPersister thumbStore)
			: base(tag, languagePreference) {

			AdditionalNames = tag.Names.AdditionalNamesString;
			Translations = tag.Names.GetTranslationsString(languagePreference);

			Albums = albums.Select(a => new AlbumContract(a, languagePreference)).ToArray();
			AlbumCount = albumCount;

			Artists = artists.Select(a => new ArtistContract(a, languagePreference)).ToArray();
			ArtistCount = artistCount;

			Description = tag.Description;
			RelatedTags = tag.RelatedTags
				.Where(t => !t.LinkedTag.Deleted)
				.Select(a => new TagBaseContract(a.LinkedTag, languagePreference, true))
				.OrderBy(t => t.Name)
				.ToArray();

			Children = tag.Children
				.Select(a => new TagBaseContract(a, languagePreference))
				.OrderBy(t => t.Name)
				.ToArray();

			EventSeries = eventSeries.Select(a => new ReleaseEventSeriesContract(a, languagePreference, false)).ToArray();
			EventSeriesCount = eventSeriesCount;

			Events = events.Select(a => new ReleaseEventForApiContract(a, languagePreference, ReleaseEventOptionalFields.AdditionalNames | ReleaseEventOptionalFields.MainPicture, thumbStore)).ToArray();
			EventCount = eventCount;

			Siblings = tag.Siblings
				.Select(a => new TagBaseContract(a, languagePreference))
				.OrderBy(t => t.Name)
				.ToArray();

			SongLists = songLists.Select(a => new SongListBaseContract(a)).ToArray();
			SongListCount = songListCount;

			Songs = songs.Select(a => new SongForApiContract(a, languagePreference, SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl)).ToArray();
			SongCount = songCount;

			Thumb = (tag.Thumb != null ? new EntryThumbContract(tag.Thumb) : null);
			WebLinks = tag.WebLinks.Links.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();
			MappedNicoTags = tag.Mappings.Select(t => t.SourceTag).ToArray();

		}

		public int AlbumCount { get; set; }

		public int AllUsageCount => ArtistCount + AlbumCount + SongCount + EventCount + SongListCount;

		public int ArtistCount { get; set; }

		public AlbumContract[] Albums { get; set; }

		public ArtistContract[] Artists { get; set; }

		public TagBaseContract[] Children { get; set; }

		public int CommentCount { get; set; }

		public new EnglishTranslatedString Description { get; set; }

		public int EventCount { get; set; }

		public int EventSeriesCount { get; set; }

		public ReleaseEventForApiContract[] Events { get; set; }

		public ReleaseEventSeriesContract[] EventSeries { get; set; }

		public int FollowerCount { get; set; }

		public bool IsFollowing { get; set; }

		public CommentForApiContract[] LatestComments { get; set; }

		public string[] MappedNicoTags { get; set; }

		public EntryType RelatedEntryType { get; set; }

		public string RelatedEntrySubType { get; set; }

		public TagBaseContract[] RelatedTags { get; set; }

		public TagBaseContract[] Siblings { get; set; }

		public int SongListCount { get; set; }

		public SongListBaseContract[] SongLists { get; set; }

		public SongForApiContract[] Songs { get; set; }

		public int SongCount { get; set; }

		public EntryThumbContract Thumb { get; set; }

		public string Translations { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

		public object JsonModel => new {
			Name, 
			Parent, 
			Children = Children.Take(20),
			Siblings = Siblings.Take(20),
			HasMoreChildren = Children.Length > 20,
			HasMoreSiblings = Siblings.Length > 20
		};

	}

}
