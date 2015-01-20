
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

		pvs: pvs.PVContract[];

		songType: string;

		status: string;

		tags: string[];

		updateNotes?: string;

		webLinks: vdb.dataContracts.WebLinkContract[];

	}

}