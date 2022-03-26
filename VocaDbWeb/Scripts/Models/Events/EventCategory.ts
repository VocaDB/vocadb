// Corresponds to the EventCategory enum in C#.
enum EventCategory {
	Unspecified = 0,
	AlbumRelease = 1 << 0,
	Anniversary = 1 << 1,
	Club = 1 << 2,
	Concert = 1 << 3,
	Contest = 1 << 4,
	Convention = 1 << 5,
	Other = 1 << 6,
}

export default EventCategory;
