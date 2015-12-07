
module vdb.dataContracts.songs {
	
	export interface SongForEditContract {

		artists: ArtistForAlbumContract[];

		defaultNameLanguage: string;

		deleted: boolean;

		id: number;

		lengthSeconds: number;

		lyrics: LyricsForSongContract[];

		names: globalization.LocalizedStringWithIdContract[];

		notes: globalization.EnglishTranslatedStringContract;

		originalVersion: SongContract;

		// Publish date, should be in ISO format, UTC timezone. Only includes the date component, no time.
		publishDate?: string;

		pvs: pvs.PVContract[];

		songType: string;

		status: string;

		tags: number[];

		updateNotes?: string;

		webLinks: vdb.dataContracts.WebLinkContract[];

	}

}