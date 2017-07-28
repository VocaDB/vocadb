namespace VocaDb.Model.Service.Search {

	/// <summary>
	/// Type of <see cref="AdvancedSearchFilter"/>.
	/// Filter types are common for all entry types, but not all filter types are supported for all entry types.
	/// </summary>
	public enum AdvancedFilterType {

		Nothing,

		// Common

		/// <summary>
		/// Song or album has artist with type specified by param.
		/// </summary>
		ArtistType,

		/// <summary>
		/// Entry has external link
		/// </summary>
		WebLink,

		// Artist

		/// <summary>
		/// Artist has verified user account on VocaDB
		/// </summary>
		HasUserAccount,

		RootVoicebank,

		/// <summary>
		/// Artist is voice provider of a voicebank whose type is specified by param.
		/// </summary>
		VoiceProvider,

		// Album

		/// <summary>
		/// Album has store (commercial) link
		/// </summary>
		HasStoreLink,

		/// <summary>
		/// Album with no cover picture
		/// </summary>
		NoCoverPicture,

		// Song

		/// <summary>
		/// Song is an album song
		/// </summary>
		HasAlbum,

		HasOriginalMedia,

		/// <summary>
		/// Song has any media.
		/// This includes disabled media (to make it more useful for searching abandoned songs).
		/// </summary>
		HasMedia,

		HasMultipleVoicebanks,

		HasPublishDate,

		/// <summary>
		/// Song has lyrics in content language specified by param.
		/// </summary>
		Lyrics,

		LyricsContent

	}

}
