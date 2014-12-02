
module vdb.dataContracts.songs {

	// Basically song with artists at the moment. Unlike SongForApiContract, this one has only real artists, and no roles.
	// Used for editing song properties in album
	export interface SongWithComponentsContract extends SongContract {

		artists?: ArtistContract[];

	}

}