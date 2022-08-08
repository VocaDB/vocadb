import ArtistType from '@/Models/Artists/ArtistType';

import EntryPictureFileContract from '../EntryPictureFileContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import LocalizedStringWithIdContract from '../Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '../WebLinkContract';
import ArtistContract from './ArtistContract';
import ArtistForArtistContract from './ArtistForArtistContract';

// Corresponds to the ArtistForEditForApiContract record class in C#.
export default interface ArtistForEditContract {
	artistType: ArtistType;
	associatedArtists: ArtistForArtistContract[];
	baseVoicebank?: ArtistContract;
	canDelete?: boolean;
	defaultNameLanguage: string;
	deleted?: boolean;
	description: EnglishTranslatedStringContract;
	groups: ArtistForArtistContract[];
	id: number;
	illustrator?: ArtistContract;
	name?: string;
	names: LocalizedStringWithIdContract[];
	pictureMime?: string;
	pictures: EntryPictureFileContract[];
	releaseDate?: string;
	status: string;
	updateNotes: string;
	voiceProvider?: ArtistContract;
	webLinks: WebLinkContract[];
}
