using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Search.SongSearch
{
	public sealed record SongListQueryParams
	{
		public bool ChildTags { get; init; }

		public SongListFeaturedCategory? FeaturedCategory { get; init; }

		public PagingProperties Paging { get; init; } = PagingProperties.Default;

		public SongListSortRule SortRule { get; init; }

		public int[]? TagIds { get; init; }

		public SearchTextQuery TextQuery { get; init; } = SearchTextQuery.Empty;
	}
}
