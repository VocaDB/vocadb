export enum TagTargetTypes {
	Nothing = 0,
	Album = 1 << 0,
	Artist = 1 << 1,
	Event = 1 << 4,
	Song = 1 << 6,
	SongList = 1 << 7,
	All = 1073741823,
}
