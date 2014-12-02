using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.User {

	public class FollowedArtistQueryParams {

		public ArtistType ArtistType { get; set; }

		/// <summary>
		/// Paging properties. Cannot be null.
		/// </summary>
		public PagingProperties Paging { get; set; }

		/// <summary>
		/// Id of the user whose artists to get.
		/// </summary>
		public int UserId { get; set; }

	}

}
