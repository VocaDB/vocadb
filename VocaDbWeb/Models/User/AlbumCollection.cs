#nullable disable

using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Models.User
{
	public class AlbumCollection
	{
		public AlbumCollection() { }

		public AlbumCollection(UserContract user, AlbumCollectionRouteParams routeParams)
		{
			User = user;
			RouteParams = routeParams;

			FilterByPurchaseStatus = routeParams.purchaseStatus ?? PurchaseStatus.Nothing;
		}

		public PurchaseStatus FilterByPurchaseStatus { get; set; }

		public AlbumCollectionRouteParams RouteParams { get; set; }

		public UserContract User { get; set; }
	}

	public class AlbumCollectionRouteParams
	{
		public int id { get; set; }

		public int? page { get; set; }

		public int? pageSize { get; set; }

		public PurchaseStatus? purchaseStatus { get; set; }

		public int totalCount { get; set; }
	}
}