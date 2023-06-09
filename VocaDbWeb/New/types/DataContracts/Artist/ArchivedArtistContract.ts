import { ArchivedEntryPictureFileContract } from '@/types/DataContracts/ArchivedEntryPictureFileContract';
import { ArchivedTranslatedStringContract } from '@/types/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/types/DataContracts/ArchivedWebLinkContract';
import { ArchivedArtistForArtistContract } from '@/types/DataContracts/Artist/ArchivedArtistForArtistContract';
import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';
import { ObjectRefContract } from '@/types/DataContracts/ObjectRefContract';
import { ArtistType } from '@/types/Models/Artists/ArtistType';

export interface ArchivedArtistContract {
	artistType: ArtistType;
	baseVoicebank?: ObjectRefContract;
	description?: string;
	descriptionEng?: string;
	groups: ArchivedArtistForArtistContract[];
	id: number;
	mainPictureMime?: string;
	members: ObjectRefContract[];
	names?: LocalizedStringContract[];
	pictures?: ArchivedEntryPictureFileContract[];
	releaseDate?: string;
	translatedName: ArchivedTranslatedStringContract;
	webLinks?: ArchivedWebLinkContract[];
}
