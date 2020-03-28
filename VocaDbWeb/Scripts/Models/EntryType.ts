
    // Identifies common entry type.
    // Corresponds to the EntryType enum C#.
    enum EntryType {
        
		Undefined			= 0,

		Album				= 1,

		Artist				= 2,

		DiscussionTopic		= 4,

		PV					= 8,

		ReleaseEvent		= 16,

		ReleaseEventSeries	= 32,

		Song				= 64,

		SongList			= 128,

		Tag					= 256,

		User                = 512,

		Venue               = 1024
		
    }

	export default EntryType;