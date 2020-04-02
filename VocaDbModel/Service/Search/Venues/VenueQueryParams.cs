using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Model.Service.Search.Venues {

	public class VenueQueryParams {

		public GeoPointQueryParams Coordinates { get; set; } = GeoPointQueryParams.Empty;

		public DistanceUnit DistanceUnit { get; set; } = DistanceUnit.Kilometers;

		public PagingProperties Paging { get; set; }

		public double? Radius { get; set; }

		public VenueSortRule SortRule { get; set; }

		public SearchTextQuery TextQuery { get; set; } = SearchTextQuery.Empty;

		public VenueQueryParams() { }

	}

}
