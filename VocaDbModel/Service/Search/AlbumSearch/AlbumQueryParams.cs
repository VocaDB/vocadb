using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.AlbumSearch {

	/// <summary>
	/// Query parameters for albums
	/// </summary>
	public class AlbumQueryParams {

		public AlbumQueryParams() {

			AlbumType = DiscType.Unknown;
			ArtistParticipationStatus = ArtistAlbumParticipationStatus.Everything;
			Common = new CommonSearchParams();
			Paging = new PagingProperties(0, 30, true);

		}

		public AlbumQueryParams(SearchTextQuery textQuery, DiscType discType, int start, int maxResults, bool draftsOnly, bool getTotalCount,
			AlbumSortRule sortRule = AlbumSortRule.Name, bool moveExactToTop = false) {

			Common = new CommonSearchParams(textQuery, draftsOnly, false, moveExactToTop);
			Paging = new PagingProperties(start, maxResults, getTotalCount);

			AlbumType = discType;
			SortRule = sortRule;

		}

		/// <summary>
		/// Album type that should be searched for. Cannot be null.
		/// If Unknown, all album types are included.
		/// </summary>
		public DiscType AlbumType { get; set; }

		public int ArtistId { get; set; }

		public ArtistAlbumParticipationStatus ArtistParticipationStatus { get; set; }

		public string Barcode { get; set; }

		public bool ChildVoicebanks { get; set; }

		public CommonSearchParams Common { get; set; }

		public bool Deleted { get; set; }

		public PagingProperties Paging { get; set; }

		public AlbumSortRule SortRule { get; set; }

		public string Tag { get; set; }

	}

}
