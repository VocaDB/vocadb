using System;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.Search.SongSearch
{
	/// <summary>
	/// Query parameters for songs
	/// </summary>
	public class SongQueryParams
	{
		private int[] ignoredIds;
		private SongType[] songTypes;

		public SongQueryParams()
		{
			Common = new CommonSearchParams();
			IgnoredIds = new int[] { };
			Paging = new PagingProperties(0, 30, true);
			SongTypes = new SongType[] { };
		}

		/// <param name="query">Query search string. Can be null or empty, in which case no filtering by name is done.</param>
		/// <param name="songTypes">Allowed song types. Can be null or empy, in which case no filtering by song type is done.</param>
		/// <param name="start">0-based order number of the first item to be returned.</param>
		/// <param name="maxResults">Maximum number of results to be returned.</param>
		/// <param name="getTotalCount">Whether to return the total number of entries matching the criteria.</param>
		/// <param name="nameMatchMode">Mode for name maching. Ignored when query string is null or empty.</param>
		/// <param name="sortRule">Sort rule for results.</param>
		/// <param name="onlyByName">Whether to search items only by name, and not for example NicoId. Ignored when query string is null or empty.</param>
		/// <param name="moveExactToTop">Whether to move exact match to the top of search results.</param>
		/// <param name="ignoredIds">List of entries to be ignored. Can be null in which case no filtering is done.</param>
		public SongQueryParams(SearchTextQuery textQuery, SongType[] songTypes, int start, int maxResults,
			bool getTotalCount, SongSortRule sortRule,
			bool onlyByName, bool moveExactToTop, int[] ignoredIds)
		{
			Common = new CommonSearchParams(textQuery, onlyByName, moveExactToTop);
			Paging = new PagingProperties(start, maxResults, getTotalCount);

			SongTypes = songTypes;
			SortRule = sortRule;
			IgnoredIds = ignoredIds;
			TimeFilter = TimeSpan.Zero;
			OnlyWithPVs = false;
		}

		public AdvancedSearchFilter[] AdvancedFilters { get; set; }

		public DateTime? AfterDate { get; set; }
		public ArtistParticipationQueryParams ArtistParticipation { get; set; } = new ArtistParticipationQueryParams();

		public string[] ArtistNames { get; set; }

		public DateTime? BeforeDate { get; set; }

		public bool ChildTags { get; set; }

		public CommonSearchParams Common { get; set; }

		/// <summary>
		/// Filter to include only songs with artists followed by the specified user.
		/// 0 = no filtering (default).
		/// </summary>
		public int FollowedByUserId { get; set; }

		/// <summary>
		/// List of songs that should be ignored from search. Cannot be null. If set to empty, will be replaced with empty list.
		/// If empty, no filtering is done by song IDs.
		/// TODO: this isn't really in use anymore. Filtering by ID should be done after search.
		/// </summary>
		public int[] IgnoredIds
		{
			get { return ignoredIds; }
			set
			{
				ignoredIds = value ?? new int[] { };
			}
		}

		public ContentLanguagePreference LanguagePreference { get; set; }

		public int MinScore { get; set; }

		public bool OnlyWithPVs { get; set; }

		public PagingProperties Paging { get; set; }

		/// <summary>
		/// ID of parent song to filter.
		/// 0 = no filter.
		/// </summary>
		public int ParentSongId { get; set; }

		public PVServices? PVServices { get; set; }

		/// <summary>
		/// Filter by release event.
		/// 0 = no filter.
		/// </summary>
		public int ReleaseEventId { get; set; }

		/// <summary>
		/// List of song types that should be searched for. Cannot be null.
		/// If empty, all song types are included.
		/// </summary>
		public SongType[] SongTypes
		{
			get { return songTypes; }
			set
			{
				songTypes = value ?? new SongType[] { };
			}
		}

		public SongSortRule SortRule { get; set; }

		public string[] Tags { get; set; }

		public int[] TagIds { get; set; }

		public TimeSpan TimeFilter { get; set; }

		/// <summary>
		/// When searching by entry type, search also by associated tag and vice versa.
		/// </summary>
		public bool UnifyEntryTypesAndTags { get; set; }

		/// <summary>
		/// Filter to include only songs rated by the specified user.
		/// 0 = no filtering (default).
		/// </summary>
		public int UserCollectionId { get; set; }
	}
}