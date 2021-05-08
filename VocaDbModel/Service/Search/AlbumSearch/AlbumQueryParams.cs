using System;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Search.AlbumSearch
{
	/// <summary>
	/// Query parameters for albums
	/// </summary>
	public sealed record AlbumQueryParams
	{
		public AlbumQueryParams()
		{
			AlbumType = DiscType.Unknown;
		}

		public AlbumQueryParams(SearchTextQuery textQuery, DiscType discType, int start, int maxResults, bool getTotalCount,
			AlbumSortRule sortRule = AlbumSortRule.Name, bool moveExactToTop = false)
		{
			Common = new CommonSearchParams(textQuery, false, moveExactToTop);
			Paging = new PagingProperties(start, maxResults, getTotalCount);

			AlbumType = discType;
			SortRule = sortRule;
		}

		public AdvancedSearchFilter[]? AdvancedFilters { get; init; }

		/// <summary>
		/// Album type that should be searched for. Cannot be null.
		/// If Unknown, all album types are included.
		/// </summary>
		public DiscType AlbumType { get; init; }

		public ArtistParticipationQueryParams ArtistParticipation { get; init; } = new();

		public string? Barcode { get; init; }

		public bool ChildTags { get; init; }

		public CommonSearchParams Common { get; init; } = CommonSearchParams.Default;

		public bool Deleted { get; init; }

		public ContentLanguagePreference LanguagePreference { get; init; }

		public PagingProperties Paging { get; init; } = PagingProperties.Default;

		public DateTime? ReleaseDateAfter { get; init; }

		public DateTime? ReleaseDateBefore { get; init; }

		public AlbumSortRule SortRule { get; init; }

		public string[]? Tags { get; init; }

		public int[]? TagIds { get; init; }
	}
}
