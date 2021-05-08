using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search.Artists;

namespace VocaDb.Model.Service.Search.User
{
	public sealed record FollowedArtistQueryParams
	{
		public ArtistType ArtistType { get; init; }

		/// <summary>
		/// Paging properties. Cannot be null.
		/// </summary>
		public PagingProperties Paging { get; init; } = PagingProperties.Default;

		public ArtistSortRule SortRule { get; init; }

		public int[]? TagIds { get; init; }

		public ArtistSearchTextQuery TextQuery { get; init; } = ArtistSearchTextQuery.Empty;

		/// <summary>
		/// Id of the user whose artists to get.
		/// </summary>
		public int UserId { get; init; }
	}
}
