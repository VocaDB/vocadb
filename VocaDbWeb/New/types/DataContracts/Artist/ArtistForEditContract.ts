import { ArtistContract } from '@/types/DataContracts/Artist/ArtistContract';
import { ArtistForArtistContract } from '@/types/DataContracts/Artist/ArtistForArtistContract';
import { EntryPictureFileContract } from '@/types/DataContracts/EntryPictureFileContract';
import { EnglishTranslatedStringContract } from '@/types/DataContracts/Globalization/EnglishTranslatedStringContract';
import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { ArtistType } from '@/types/Models/Artists/ArtistType';
import { EntryStatus } from '@/types/Models/EntryStatus';

// Corresponds to the ArtistForEditForApiContract record class in C#.
export interface ArtistForEditContract {
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
	status: EntryStatus;
	updateNotes: string;
	voiceProvider?: ArtistContract;
	webLinks: WebLinkContract[];
	cultureCodes: string[];
}
