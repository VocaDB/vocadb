using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.SongSearch {

	public class SongListQueryParams {

		public int ListId { get; set; }

		public PagingProperties Paging { get; set; }

		public PVServices? PVServices { get; set; }

	}

}
