using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.SongSearch {

	public class SongListQueryParams {

		public int[] ArtistIds { get; set; }

		public bool ChildVoicebanks { get; set; }

		public int ListId { get; set; }

		public SearchTextQuery TextQuery { get; set; }

		public PagingProperties Paging { get; set; }

		public PVServices? PVServices { get; set; }

		/// <summary>
		/// Song sort rule. If null, Order field will be used.
		/// </summary>
		public SongSortRule? SortRule { get; set; }

		public int[] TagIds { get; set; }

	}

}
