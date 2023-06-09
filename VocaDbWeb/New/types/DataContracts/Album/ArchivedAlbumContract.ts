import { AlbumDiscPropertiesContract } from '@/types/DataContracts/Album/AlbumDiscPropertiesContract';
import { AlbumIdentifierContract } from '@/types/DataContracts/Album/AlbumIdentifierContract';
import { ArchivedAlbumReleaseContract } from '@/types/DataContracts/Album/ArchivedAlbumReleaseContract';
import { ArchivedArtistForAlbumContract } from '@/types/DataContracts/Album/ArchivedArtistForAlbumContract';
import { SongInAlbumRefContract } from '@/types/DataContracts/Album/SongInAlbumRefContract';
import { ArchivedEntryPictureFileContract } from '@/types/DataContracts/ArchivedEntryPictureFileContract';
import { ArchivedTranslatedStringContract } from '@/types/DataContracts/ArchivedTranslatedStringContract';
import { ArchivedWebLinkContract } from '@/types/DataContracts/ArchivedWebLinkContract';
import { LocalizedStringContract } from '@/types/DataContracts/Globalization/LocalizedStringContract';
import { ArchivedPVContract } from '@/types/DataContracts/PVs/ArchivedPVContract';
import { AlbumType } from '@/types/Models/Albums/AlbumType';

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
