namespace VocaDb.Model.Domain {

	public enum EntryType {

		Undefined			= 0,

		Album				= 1,

		Artist				= 2,

		PV					= 4,

		ReleaseEvent		= 8,

		ReleaseEventSeries	= 16,

		Song				= 32,

		SongList			= 64,

		Tag					= 128,

		User				= 256

	}
}
