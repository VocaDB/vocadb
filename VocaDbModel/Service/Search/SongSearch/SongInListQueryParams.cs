using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.SongSearch
{
	public class SongInListQueryParams
	{
		public AdvancedSearchFilter[] AdvancedFilters { get; set; }

		public int[] ArtistIds { get; set; }

		public bool ChildVoicebanks { get; set; }

		public int ListId { get; set; }

		public SearchTextQuery TextQuery { get; set; }

		public PagingProperties Paging { get; set; } = new PagingProperties(0, 30, true);

		public PVServices? PVServices { get; set; }

		/// <summary>
		/// Song sort rule. If null, Order field will be used.
		/// </summary>
		public SongSortRule? SortRule { get; set; }

		public SongType[] SongTypes { get; set; }

		public int[] TagIds { get; set; }
	}
}
