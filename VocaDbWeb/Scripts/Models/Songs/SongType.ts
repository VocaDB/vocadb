enum SongType {
	Unspecified = 0,

	Original = 1 << 0,

	Remaster = 1 << 1,

	Remix = 1 << 2,

	Cover = 1 << 3,

	Arrangement = 1 << 4,

	Instrumental = 1 << 5,

	Mashup = 1 << 6,

	MusicPV = 1 << 7,

	DramaPV = 1 << 8,

	Live = 1 << 9,

	Illustration = 1 << 10,

	Other = 1 << 11,
}

export default SongType;
