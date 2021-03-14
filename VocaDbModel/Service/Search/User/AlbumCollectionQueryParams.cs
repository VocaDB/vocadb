#nullable disable

using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.User
{
	public class AlbumCollectionQueryParams
	{
#nullable enable
		public AlbumCollectionQueryParams(int userId, PagingProperties paging)
		{
			ParamIs.NotNull(() => paging);

			Paging = paging;
			UserId = userId;

			FilterByStatus = null;
			Sort = AlbumSortRule.Name;
			TextQuery = new SearchTextQuery();
		}
#nullable disable

		public AdvancedSearchFilter[] AdvancedFilters { get; set; }

		public DiscType AlbumType { get; set; }

		public int ArtistId { get; set; }

		public PurchaseStatus[] FilterByStatus { get; set; }

		public SearchTextQuery TextQuery { get; set; }

		/// <summary>
		/// Paging properties. Cannot be null.
		/// </summary>
		public PagingProperties Paging { get; set; }

		public int ReleaseEventId { get; set; }

		public AlbumSortRule Sort { get; set; }

		public string Tag { get; set; }

		public int TagId { get; set; }

		/// <summary>
		/// Id of the user whose albums to get.
		/// </summary>
		public int UserId { get; set; }
	}
}
