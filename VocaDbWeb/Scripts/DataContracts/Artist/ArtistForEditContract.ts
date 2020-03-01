import ArtistContract from './ArtistContract';
import ArtistForArtistContract from './ArtistForArtistContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import EntryPictureFileContract from '../EntryPictureFileContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '../WebLinkContract';

//module vdb.dataContracts.artists {
	
	export default interface ArtistForEditContract {

		artistType: string;

		associatedArtists: ArtistForArtistContract[];

		baseVoicebank: ArtistContract;

		defaultNameLanguage: string;

		description: EnglishTranslatedStringContract;

		groups: ArtistForArtistContract[];

		id: number;

		illustrator: ArtistContract;

		names: LocalizedStringWithIdContract[];

		pictureMime: string;

		pictures: EntryPictureFileContract[];

		releaseDate?: string;

		status: string;

		updateNotes: string;

		voiceProvider: ArtistContract;

		webLinks: WebLinkContract[];

	}

//}