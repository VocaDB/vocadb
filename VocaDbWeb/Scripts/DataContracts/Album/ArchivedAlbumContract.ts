import { AlbumDiscPropertiesContract } from '@/DataContracts/Album/AlbumDiscPropertiesContract';
import { AlbumIdentifierContract } from '@/DataContracts/Album/AlbumIdentifierContract';
import { ArchivedAlbumReleaseContract } from '@/DataContracts/Album/ArchivedAlbumReleaseContract';
import { ArchivedArtistForAlbumContract } from '@/DataContracts/Album/ArchivedArtistForAlbumContract';
import { SongInAlbumRefContract } from '@/DataContracts/Album/SongInAlbumRefContract';
import { ArchivedEntryPictureFileContract } from '@/DataContracts/ArchivedEntryPictureFileContract';
import { ArchivedTranslatedStringContract } from '@/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';
import { ArchivedPVContract } from '@/DataContracts/PVs/ArchivedPVContract';
import { AlbumType } from '@/Models/Albums/AlbumType';

export interface ArchivedAlbumContract {
	artists?: ArchivedArtistForAlbumContract[];
	description?: string;
	descriptionEng?: string;
	discs?: AlbumDiscPropertiesContract[];
	discType: AlbumType;
	id: number;
	identifiers: AlbumIdentifierContract[];
	mainPictureMime?: string;
	names?: LocalizedStringContract[];
	originalRelease?: ArchivedAlbumReleaseContract;
	pictures?: ArchivedEntryPictureFileContract[];
	pvs?: ArchivedPVContract[];
	songs?: SongInAlbumRefContract[];
	translatedName: ArchivedTranslatedStringContract;
	webLinks?: ArchivedWebLinkContract[];
}
