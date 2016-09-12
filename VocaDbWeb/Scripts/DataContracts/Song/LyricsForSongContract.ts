
module vdb.dataContracts.songs {
	
	export interface LyricsForSongContract {

		cultureCode?: string;

		id?: number;

		language?: string;

		source?: string;

		translationType: string;

		url?: string;

		value?: string;

	}

}