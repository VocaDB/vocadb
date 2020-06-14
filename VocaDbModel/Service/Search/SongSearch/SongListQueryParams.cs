using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Model.Service.Search.SongSearch {

	public class SongListQueryParams {

		public bool ChildTags { get; set; }

		public SongListFeaturedCategory? FeaturedCategory { get; set; }

		public PagingProperties Paging { get; set; } = new PagingProperties(0, 30, true);

		public SongListSortRule SortRule { get; set; }

		public int[] TagIds { get; set; }

		public SearchTextQuery TextQuery { get; set; }

	}

}
