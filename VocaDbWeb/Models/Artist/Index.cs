using VocaDb.Model;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service;

namespace VocaDb.Web.Models.Artist {

	/// <summary>
	/// Parameter collection given to index action.
	/// </summary>
	public class IndexRouteParams {

		public IndexRouteParams() { }

		public IndexRouteParams(IndexRouteParams source, int? page) {

			ParamIs.NotNull(() => source);

			artistType = (source.artistType != ArtistType.Unknown ? source.artistType : null);
			draftsOnly = (source.draftsOnly == true ? source.draftsOnly : null);
			filter = source.filter;
			matchMode = source.matchMode;
			sort = source.sort;
			this.page = page;

		}

		public ArtistType? artistType { get; set; }
		public bool? draftsOnly { get; set; }
		public string filter { get; set; }
		public NameMatchMode? matchMode { get; set; }
		public int? page { get; set; }
		public ArtistSortRule? sort { get; set; }

	}

}