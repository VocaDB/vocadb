using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.User {

	public class AlbumCollectionQueryParams {

		public AlbumCollectionQueryParams(int userId, PagingProperties paging) {

			ParamIs.NotNull(() => paging);

			Paging = paging;
			UserId = userId;

			FilterByStatus = null;
			NameMatchMode = NameMatchMode.Auto;
			Query = string.Empty;
			Sort = AlbumSortRule.Name;

		}

		public int ArtistId { get; set; }

		public PurchaseStatus[] FilterByStatus { get; set; }

		public NameMatchMode NameMatchMode { get; set; }

		/// <summary>
		/// Paging properties. Cannot be null.
		/// </summary>
		public PagingProperties Paging { get; set; }

		public string Query { get; set; }

		public string ReleaseEventName { get; set; }

		public AlbumSortRule Sort { get; set; }

		public string Tag { get; set; }

		/// <summary>
		/// Id of the user whose albums to get.
		/// </summary>
		public int UserId { get; set; }


	}

}
