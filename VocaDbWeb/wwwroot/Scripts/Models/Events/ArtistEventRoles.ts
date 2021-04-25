
	enum ArtistEventRoles {
		
		Default = 0,

		Dancer = 1 << 0,

		/// <summary>
		/// Disc jockey (plays songs)
		/// </summary>
		DJ = 1 << 1,

		/// <summary>
		/// Plays an instrument
		/// </summary>
		Instrumentalist = 1 << 2,

		/// <summary>
		/// Organizes event (might not participate directly)
		/// </summary>
		Organizer = 1 << 3,

		/// <summary>
		/// Promotes (advertises) event (might not participate directly)
		/// </summary>
		Promoter = 1 << 4,

		/// <summary>
		/// Video jockey (plays videos)
		/// </summary>
		VJ = 1 << 5,

		Vocalist = 1 << 6,

		/// <summary>
		/// Voice manipulator of Vocaloid/UTAU
		/// </summary>
		VoiceManipulator = 1 << 7,

		OtherPerformer = 1 << 8,

		Other = 1 << 9,

	}

	export default ArtistEventRoles;