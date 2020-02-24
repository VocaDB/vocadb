using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.Venues {

	public class VenueQueryParams {

		public PagingProperties Paging { get; set; }

		public SearchTextQuery TextQuery { get; set; } = SearchTextQuery.Empty;

		public VenueQueryParams() { }

	}

}
