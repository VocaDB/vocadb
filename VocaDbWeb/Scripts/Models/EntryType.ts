// Identifies common entry type.
// Corresponds to the EntryType enum C#.
enum EntryType {
	Undefined = 0,

	Album = 1 << 0,

	Artist = 1 << 1,

	DiscussionTopic = 1 << 2,

	PV = 1 << 3,

	ReleaseEvent = 1 << 4,

	ReleaseEventSeries = 1 << 5,

	Song = 1 << 6,

	SongList = 1 << 7,

	Tag = 1 << 8,

	User = 1 << 9,

	Venue = 1 << 10,
}

export default EntryType;
