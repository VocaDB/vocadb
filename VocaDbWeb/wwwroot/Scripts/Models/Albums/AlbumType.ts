
	enum AlbumType {
		
		Unknown = 0,

		Album = 1 << 0,

		Single = 1 << 1,

		EP = 1 << 2,

		SplitAlbum = 1 << 3,

		Compilation = 1 << 4,

		Video = 1 << 5,

		Artbook = 1 << 6,

		Game = 1 << 7,

		Fanmade = 1 << 8,

		Instrumental = 1 << 9,

		Other = 1 << 10,

	}

	export default AlbumType;