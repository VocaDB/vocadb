using System;
using System.Diagnostics.CodeAnalysis;
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
	public sealed record SongQueryParams
	{
		private int[] _ignoredIds;
		private SongType[] _songTypes;

		public SongQueryParams()
		{
			IgnoredIds = new int[] { };
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
		public SongQueryParams(SearchTextQuery textQuery, SongType[]? songTypes, int start, int maxResults,
			bool getTotalCount, SongSortRule sortRule,
			bool onlyByName, bool moveExactToTop, int[]? ignoredIds)
		{
			Common = new CommonSearchParams(textQuery, onlyByName, moveExactToTop);
			Paging = new PagingProperties(start, maxResults, getTotalCount);

			SongTypes = songTypes;
			SortRule = sortRule;
			IgnoredIds = ignoredIds;
			TimeFilter = TimeSpan.Zero;
			OnlyWithPVs = false;
		}

		public AdvancedSearchFilter[]? AdvancedFilters { get; init; }

		public DateTime? AfterDate { get; init; }
		public ArtistParticipationQueryParams ArtistParticipation { get; init; } = new();

		public string[]? ArtistNames { get; init; }

		public DateTime? BeforeDate { get; init; }

		public bool ChildTags { get; init; }

		public CommonSearchParams Common { get; init; } = CommonSearchParams.Default;

		/// <summary>
		/// Filter to include only songs with artists followed by the specified user.
		/// 0 = no filtering (default).
		/// </summary>
		public int FollowedByUserId { get; init; }

		/// <summary>
		/// List of songs that should be ignored from search. Cannot be null. If set to empty, will be replaced with empty list.
		/// If empty, no filtering is done by song IDs.
		/// TODO: this isn't really in use anymore. Filtering by ID should be done after search.
		/// </summary>
		[AllowNull]
		public int[] IgnoredIds
		{
			get => _ignoredIds;
			[MemberNotNull(nameof(_ignoredIds))]
			init
			{
				_ignoredIds = value ?? new int[] { };
			}
		}

		public ContentLanguagePreference LanguagePreference { get; init; }

		public int MinScore { get; init; }

		public bool OnlyWithPVs { get; init; }

		public PagingProperties Paging { get; init; } = PagingProperties.Default;

		/// <summary>
		/// ID of parent song to filter.
		/// 0 = no filter.
		/// </summary>
		public int ParentSongId { get; init; }

		public PVServices? PVServices { get; init; }

		/// <summary>
		/// Filter by release event.
		/// 0 = no filter.
		/// </summary>
		public int ReleaseEventId { get; init; }

		/// <summary>
		/// List of song types that should be searched for. Cannot be null.
		/// If empty, all song types are included.
		/// </summary>
		[AllowNull]
		public SongType[] SongTypes
		{
			get => _songTypes;
			[MemberNotNull(nameof(_songTypes))]
			init
			{
				_songTypes = value ?? new SongType[] { };
			}
		}

		public SongSortRule SortRule { get; init; }

		public string[]? Tags { get; init; }

		public int[]? TagIds { get; init; }

		public TimeSpan TimeFilter { get; init; }

		/// <summary>
		/// When searching by entry type, search also by associated tag and vice versa.
		/// </summary>
		public bool UnifyEntryTypesAndTags { get; init; }

		/// <summary>
		/// Filter to include only songs rated by the specified user.
		/// 0 = no filtering (default).
		/// </summary>
		public int UserCollectionId { get; init; }

		public int? MinMilliBpm { get; init; }

		public int? MaxMilliBpm { get; init; }

		public int? MinLength { get; init; }

		public int? MaxLength { get; init; }
	}
}