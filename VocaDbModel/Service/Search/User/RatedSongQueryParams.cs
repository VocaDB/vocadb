using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Search.User
{
	/// <summary>
	/// Query parameters for rated (favorited/liked) songs by user.
	/// </summary>
	public sealed record RatedSongQueryParams
	{
		public RatedSongQueryParams(int userId, PagingProperties paging)
		{
			ParamIs.NotNull(() => paging);

			Paging = paging;
			UserId = userId;

			FilterByRating = SongVoteRating.Nothing;
			GroupByRating = true;
			SortRule = RatedSongForUserSortRule.Name;
		}

		public AdvancedSearchFilter[]? AdvancedFilters { get; init; }

		public LogicalGrouping ArtistGrouping { get; init; }

		public int[]? ArtistIds { get; init; }

		public bool ChildVoicebanks { get; init; }

		public SongVoteRating FilterByRating { get; init; }

		/// <summary>
		/// Group by rating.
		/// </summary>
		public bool GroupByRating { get; init; }

		/// <summary>
		/// Paging properties. Cannot be null.
		/// </summary>
		public PagingProperties Paging { get; init; }

		public PVServices? PVServices { get; init; }

		public int SonglistId { get; init; }

		/// <summary>
		/// Song sort rule.
		/// </summary>
		public RatedSongForUserSortRule SortRule { get; init; }

		public string? TagName { get; init; }

		public int[]? TagIds { get; init; }

		public SearchTextQuery TextQuery { get; init; } = SearchTextQuery.Empty;

		/// <summary>
		/// Id of the user whose songs to get.
		/// </summary>
		public int UserId { get; init; }
	}
}
