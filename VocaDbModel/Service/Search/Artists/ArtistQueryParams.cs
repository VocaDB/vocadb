using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.Artists
{
	/// <summary>
	/// Query parameters for artists
	/// </summary>
	public sealed record ArtistQueryParams
	{
		public ArtistQueryParams()
		{
			ArtistTypes = new ArtistType[] { };
		}

		/// <param name="query">Query search string. Can be null or empty, in which case no filtering by name is done.</param>
		/// <param name="songTypes">Allowed song types. Can be null or empy, in which case no filtering by song type is done.</param>
		/// <param name="start">0-based order number of the first item to be returned.</param>
		/// <param name="maxResults">Maximum number of results to be returned.</param>
		/// <param name="getTotalCount">Whether to return the total number of entries matching the criteria.</param>
		/// <param name="nameMatchMode">Mode for name maching. Ignored when query string is null or empty.</param>
		/// <param name="sortRule">Sort rule for results.</param>
		/// <param name="moveExactToTop">Whether to move exact match to the top of search results.</param>
		public ArtistQueryParams(ArtistSearchTextQuery textQuery, ArtistType[]? songTypes, int start, int maxResults,
			bool getTotalCount, ArtistSortRule sortRule, bool moveExactToTop)
		{
			Common = CommonSearchParams.Create(textQuery, true, moveExactToTop);
			Paging = new PagingProperties(start, maxResults, getTotalCount);

			ArtistTypes = songTypes ?? new ArtistType[] { };
			SortRule = sortRule;
		}

		public AdvancedSearchFilter[]? AdvancedFilters { get; init; }

		public bool AllowBaseVoicebanks { get; init; } = true;

		public ArtistType[] ArtistTypes { get; init; }

		public bool ChildTags { get; init; }

		public CommonSearchParams<ArtistSearchTextQuery> Common { get; init; } = CommonSearchParams<ArtistSearchTextQuery>.Default;

		public ContentLanguagePreference LanguagePreference { get; init; }

		public PagingProperties Paging { get; init; } = PagingProperties.Default;

		public ArtistSortRule SortRule { get; init; }

		public string[]? Tags { get; init; }

		public int[]? TagIds { get; init; }

		public int UserFollowerId { get; init; }
	}
}
