
module vdb.dataContracts.artists {
	
	export interface ArtistForEditContract {

		artistType: string;

		baseVoicebank: ArtistContract;

		defaultNameLanguage: string;

		description: string;

		groups: GroupForArtistContract[];

		id: number;

		names: globalization.LocalizedStringWithIdContract[];

		pictureMime: string;

		pictures: EntryPictureFileContract[];

		status: string;

		updateNotes: string;

		webLinks: vdb.dataContracts.WebLinkContract[];

	}

}