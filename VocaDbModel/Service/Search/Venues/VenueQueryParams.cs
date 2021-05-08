using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Search.Venues
{
	public sealed record VenueQueryParams
	{
		public GeoPointQueryParams Coordinates { get; init; } = GeoPointQueryParams.Empty;

		public DistanceUnit DistanceUnit { get; init; } = DistanceUnit.Kilometers;

		public PagingProperties Paging { get; init; } = PagingProperties.Default;

		public double? Radius { get; init; }

		public VenueSortRule SortRule { get; init; }

		public SearchTextQuery TextQuery { get; init; } = SearchTextQuery.Empty;

		public VenueQueryParams() { }
	}
}
