import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { ArtistForArtistContract } from '@/DataContracts/Artist/ArtistForArtistContract';
import { EntryPictureFileContract } from '@/DataContracts/EntryPictureFileContract';
import { EnglishTranslatedStringContract } from '@/DataContracts/Globalization/EnglishTranslatedStringContract';
import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { EntryStatus } from '@/Models/EntryStatus';

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
}
