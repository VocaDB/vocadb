using System;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Utils.Search
{
	[Obsolete]
	public enum SearchType
	{
		Anything = 0,

		Album = 1 << 0,

		Artist = 1 << 1,

		DiscussionTopic = 1 << 2,

		PV = 1 << 3,

		ReleaseEvent = 1 << 4,

		ReleaseEventSeries = 1 << 5,

		Song = 1 << 6,

		SongList = 1 << 7,

		Tag = 1 << 8,

		User = 1 << 9,

		Venue = 1 << 10,
	}

	[Obsolete]
	public class SearchRouteParams
	{
		public SearchRouteParams(SearchType? searchType = null)
		{
			this.searchType = searchType;
		}

		public int? artistId { get; set; }

		public ArtistType? artistType { get; set; }

		public DiscType? discType { get; set; }

		public EventCategory? eventCategory { get; set; }

		public int? eventId { get; set; }

		public string? filter { get; set; }

		public bool? onlyRatedSongs { get; set; }

		public bool? onlyWithPVs { get; set; }

		public SearchType? searchType { get; set; }

		public SongType? songType { get; set; }

		public object? sort { get; set; }

		public string? tag { get; set; }

		public int? tagId { get; set; }
	}

	public class SearchRouteParamsFactory
	{
		public static SearchRouteParamsFactory Instance => new SearchRouteParamsFactory();

		public SearchRouteParams Albums(int? tagId = null)
		{
			return new SearchRouteParams(SearchType.Album)
			{
				tagId = tagId,
			};
		}

		public SearchRouteParams Artists(int? tagId = null)
		{
			return new SearchRouteParams(SearchType.Artist)
			{
				tagId = tagId,
			};
		}

		public SearchRouteParams Entries(int? tagId = null)
		{
			return new SearchRouteParams(SearchType.Anything)
			{
				tagId = tagId,
			};
		}

		public SearchRouteParams Events(
			int? tagId = null,
			int? artistId = null,
			EventSortRule? sort = null,
			EventCategory? category = null)
		{
			return new SearchRouteParams(SearchType.ReleaseEvent)
			{
				tagId = tagId,
				artistId = artistId,
				sort = sort,
				eventCategory = category
			};
		}

		public SearchRouteParams Songs(
			int? artistId = null,
			int? eventId = null,
			SongType? songType = null,
			object? sort = null,
			int? tagId = null)
		{
			return new SearchRouteParams(SearchType.Song)
			{
				artistId = artistId,
				eventId = eventId,
				songType = songType,
				sort = sort,
				tagId = tagId
			};
		}
	}
}
