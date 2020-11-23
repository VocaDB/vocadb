using System;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Model.Service.Search.AlbumSearch
{
	/// <summary>
	/// Query parameters for albums
	/// </summary>
	public class AlbumQueryParams
	{
		public AlbumQueryParams()
		{
			AlbumType = DiscType.Unknown;
			Common = new CommonSearchParams();
			Paging = new PagingProperties(0, 30, true);
		}

		public AlbumQueryParams(SearchTextQuery textQuery, DiscType discType, int start, int maxResults, bool getTotalCount,
			AlbumSortRule sortRule = AlbumSortRule.Name, bool moveExactToTop = false)
		{
			Common = new CommonSearchParams(textQuery, false, moveExactToTop);
			Paging = new PagingProperties(start, maxResults, getTotalCount);

			AlbumType = discType;
			SortRule = sortRule;
		}

		public AdvancedSearchFilter[] AdvancedFilters { get; set; }

		/// <summary>
		/// Album type that should be searched for. Cannot be null.
		/// If Unknown, all album types are included.
		/// </summary>
		public DiscType AlbumType { get; set; }

		public ArtistParticipationQueryParams ArtistParticipation { get; set; } = new ArtistParticipationQueryParams();

		public string Barcode { get; set; }

		public bool ChildTags { get; set; }

		public CommonSearchParams Common { get; set; }

		public bool Deleted { get; set; }

		public ContentLanguagePreference LanguagePreference { get; set; }

		public PagingProperties Paging { get; set; }

		public DateTime? ReleaseDateAfter { get; set; }

		public DateTime? ReleaseDateBefore { get; set; }

		public AlbumSortRule SortRule { get; set; }

		public string[] Tags { get; set; }

		public int[] TagIds { get; set; }
	}
}
