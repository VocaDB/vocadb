using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.User
{
	public sealed record AlbumCollectionQueryParams
	{
		public AlbumCollectionQueryParams(int userId, PagingProperties paging)
		{
			ParamIs.NotNull(() => paging);

			Paging = paging;
			UserId = userId;

			FilterByStatus = null;
			Sort = AlbumSortRule.Name;
		}

		public AdvancedSearchFilter[]? AdvancedFilters { get; init; }

		public DiscType AlbumType { get; init; }

		public int ArtistId { get; init; }

		public PurchaseStatus[]? FilterByStatus { get; init; }

		public SearchTextQuery TextQuery { get; init; } = SearchTextQuery.Empty;

		/// <summary>
		/// Paging properties. Cannot be null.
		/// </summary>
		public PagingProperties Paging { get; init; }

		public int ReleaseEventId { get; init; }

		public AlbumSortRule Sort { get; init; }

		public string? Tag { get; init; }

		public int TagId { get; init; }

		/// <summary>
		/// Id of the user whose albums to get.
		/// </summary>
		public int UserId { get; init; }
	}
}
