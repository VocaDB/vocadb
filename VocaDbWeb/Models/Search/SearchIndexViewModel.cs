using System;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Web.Models.Search
{
	[Obsolete]
	public class SearchIndexViewModel
	{
		public SearchIndexViewModel()
			: this(EntryType.Undefined) { }

		public SearchIndexViewModel(EntryType searchType, string? filter = null)
		{
			allowRedirect = true;
			this.searchType = searchType;
			this.filter = filter;
		}

		public bool allowRedirect { get; set; }

		public int[]? artistId { get; set; }

		public ArtistType? artistType { get; set; }

		public bool? autoplay { get; set; }

		public bool? childTags { get; set; }

		public bool? childVoicebanks { get; set; }

		public DiscType? discType { get; set; }

		public EventCategory? eventCategory { get; set; }

		public int? eventId { get; set; }

		public string? filter { get; set; }

		public int? minScore { get; set; }

		public bool? onlyRatedSongs { get; set; }

		public bool? onlyWithPVs { get; set; }

		public int? pageSize { get; set; }

		public EntryType searchType { get; set; }

		public string searchTypeName => searchType != EntryType.Undefined ? searchType.ToString() : "Anything";

		public bool? shuffle { get; set; }

		public int? since { get; set; }

		public string? tag { get; set; }

		public int[]? tagId { get; set; }

		public SongType? songType { get; set; }

		public string? sort { get; set; }

		public string? viewMode { get; set; }
	}
}