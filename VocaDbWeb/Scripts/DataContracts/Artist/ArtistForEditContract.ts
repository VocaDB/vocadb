
module vdb.dataContracts.artists {
	
	export interface ArtistForEditContract {

		artistType: string;

		baseVoicebank: ArtistContract;

		defaultNameLanguage: string;

		description: globalization.EnglishTranslatedStringContract;

		groups: ArtistForArtistContract[];

		id: number;

		illustrator: ArtistContract;

		names: globalization.LocalizedStringWithIdContract[];

		pictureMime: string;

		pictures: EntryPictureFileContract[];

		status: string;

		updateNotes: string;

		voiceProvider: ArtistContract;

		webLinks: vdb.dataContracts.WebLinkContract[];

	}

}