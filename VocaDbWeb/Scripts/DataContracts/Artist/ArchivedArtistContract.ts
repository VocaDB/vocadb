import { ArchivedEntryPictureFileContract } from '@/DataContracts/ArchivedEntryPictureFileContract';
import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/DataContracts/ArchivedWebLinkContract';
import { ArchivedArtistForArtistContract } from '@/DataContracts/Artist/ArchivedArtistForArtistContract';
import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';
import { ObjectRefContract } from '@/DataContracts/ObjectRefContract';
import { ArtistType } from '@/Models/Artists/ArtistType';

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
