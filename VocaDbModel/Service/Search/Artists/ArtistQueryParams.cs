using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.Artists {

	/// <summary>
	/// Query parameters for artists
	/// </summary>
	public class ArtistQueryParams {

		public ArtistQueryParams() {

			Common = new CommonSearchParams<ArtistSearchTextQuery>();
			Paging = new PagingProperties(0, 30, true);
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
		public ArtistQueryParams(ArtistSearchTextQuery textQuery, ArtistType[] songTypes, int start, int maxResults,
			bool getTotalCount, ArtistSortRule sortRule, bool moveExactToTop) {

			Common = CommonSearchParams.Create(textQuery, true, moveExactToTop);
			Paging = new PagingProperties(start, maxResults, getTotalCount);

			ArtistTypes = songTypes ?? new ArtistType[] { };
			SortRule = sortRule;

		}

		public ArtistType[] ArtistTypes { get; set; }

		public bool ChildTags { get; set; }

		public CommonSearchParams<ArtistSearchTextQuery> Common { get; set; }

		public PagingProperties Paging { get; set; }

		public ArtistSortRule SortRule { get; set; }

		public string[] Tags { get; set; }

		public int[] TagIds { get; set; }

		public int UserFollowerId { get; set; }

	}

}
