namespace VocaDb.Model.Service.Search {

	/// <summary>
	/// Type of <see cref="AdvancedSearchFilter"/>.
	/// Filter types are common for all entry types, but not all filter types are supported for all entry types.
	/// </summary>
	public enum AdvancedFilterType {

		Nothing,

		/// <summary>
		/// Song or album has artist with type specified by param.
		/// </summary>
		ArtistType,		

		/// <summary>
		/// Song has lyrics in content language specified by param.
		/// </summary>
		Lyrics,

		/// <summary>
		/// Album with no cover picture
		/// </summary>
		NoCoverPicture,

		/// <summary>
		/// Artist is voice provider of a voicebank whose type is specified by param.
		/// </summary>
		VoiceProvider

	}

}
