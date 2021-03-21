#nullable disable

using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Search.SongSearch
{
	public class SongListQueryParams
	{
		public bool ChildTags { get; set; }

		public SongListFeaturedCategory? FeaturedCategory { get; set; }

		public PagingProperties Paging { get; set; } = new(0, 30, true);

		public SongListSortRule SortRule { get; set; }

#nullable enable
		public int[]? TagIds { get; set; }
#nullable disable

		public SearchTextQuery TextQuery { get; set; }
	}
}
