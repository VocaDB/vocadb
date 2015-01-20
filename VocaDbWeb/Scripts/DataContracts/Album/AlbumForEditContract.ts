
module vdb.dataContracts.albums {
	
	export interface AlbumForEditContract {

		artistLinks: ArtistForAlbumContract[];

		coverPictureMime?: string;

		defaultNameLanguage: string;

		description: globalization.EnglishTranslatedStringContract;

		discType: string;

		id: number;

		identifiers: string[];

		names: globalization.LocalizedStringWithIdContract[];

		originalRelease: AlbumReleaseContract;

		pictures: EntryPictureFileContract[];

		pvs: pvs.PVContract[];

		songs: songs.SongInAlbumEditContract[];

		status: string;

		updateNotes?: string;

		webLinks: WebLinkContract[];

	}

}