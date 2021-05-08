#nullable disable

using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.SongSearch
{
	public sealed record SongInListQueryParams
	{
#nullable enable
		public AdvancedSearchFilter[]? AdvancedFilters { get; init; }

		public int[]? ArtistIds { get; init; }

		public bool ChildVoicebanks { get; init; }

		public int ListId { get; init; }

		public SearchTextQuery TextQuery { get; init; } = SearchTextQuery.Empty;

		public PagingProperties Paging { get; init; } = PagingProperties.Default;

		public PVServices? PVServices { get; init; }

		/// <summary>
		/// Song sort rule. If null, Order field will be used.
		/// </summary>
		public SongSortRule? SortRule { get; init; }
#nullable disable

		public SongType[] SongTypes { get; init; }

#nullable enable
		public int[]? TagIds { get; init; }
#nullable disable
	}
}
