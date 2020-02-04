
//namespace vdb.models.events {
	
	enum ArtistEventRoles {
		
		Default = 0,

		Dancer = 1,

		/// <summary>
		/// Disc jockey (plays songs)
		/// </summary>
		DJ = 2,

		/// <summary>
		/// Plays an instrument
		/// </summary>
		Instrumentalist = 4,

		/// <summary>
		/// Organizes event (might not participate directly)
		/// </summary>
		Organizer = 8,

		/// <summary>
		/// Promotes (advertises) event (might not participate directly)
		/// </summary>
		Promoter = 16,

		/// <summary>
		/// Video jockey (plays videos)
		/// </summary>
		VJ = 32,

		Vocalist = 64,

		/// <summary>
		/// Voice manipulator of Vocaloid/UTAU
		/// </summary>
		VoiceManipulator = 128,

		OtherPerformer = 256,

		Other = 512

	}

	export default ArtistEventRoles;

//}