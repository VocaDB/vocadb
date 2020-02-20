namespace VocaDb.Model.Service.Search.AlbumSearch {

	public enum ArtistAlbumParticipationStatus {

		/// <summary>
		/// Get content regardless of participation status.
		/// </summary>
		Everything,

		/// <summary>
		/// Get only main content (direct major participation, no support).
		/// </summary>
		OnlyMainAlbums,

		/// <summary>
		/// Get only collaboration content (shared participation or support role).
		/// </summary>
		OnlyCollaborations

	}

}
