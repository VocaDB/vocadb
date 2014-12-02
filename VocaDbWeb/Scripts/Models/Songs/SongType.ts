
module vdb.models.songs {
	
	export enum SongType {
		
		Unspecified		= 0,

		Original		= 1,

		Remaster		= 2,

		Remix			= 4,

		Cover			= 8,

		Instrumental	= 16,

		Mashup			= 32,

		MusicPV			= 64,

		DramaPV			= 128,

		Live			= 256,

		Other			= 512

	}

} 