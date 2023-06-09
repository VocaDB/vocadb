import { AlbumDiscPropertiesContract } from '@/types/DataContracts/Album/AlbumDiscPropertiesContract';
import { AlbumReleaseContract } from '@/types/DataContracts/Album/AlbumReleaseContract';
import { ArtistForAlbumContract } from '@/types/DataContracts/ArtistForAlbumContract';
import { EntryPictureFileContract } from '@/types/DataContracts/EntryPictureFileContract';
import { EnglishTranslatedStringContract } from '@/types/DataContracts/Globalization/EnglishTranslatedStringContract';
import { LocalizedStringWithIdContract } from '@/types/DataContracts/Globalization/LocalizedStringWithIdContract';
import { PVContract } from '@/types/DataContracts/PVs/PVContract';
import { SongInAlbumEditContract } from '@/types/DataContracts/Song/SongInAlbumEditContract';
import { WebLinkContract } from '@/types/DataContracts/WebLinkContract';
import { AlbumType } from '@/types/Models/Albums/AlbumType';
import { EntryStatus } from '@/types/Models/EntryStatus';

// Corresponds to the AlbumForEditForApiContract record class in C#.
export interface AlbumForEditContract {
	artistLinks: ArtistForAlbumContract[];
	canDelete?: boolean;
	coverPictureMime?: string;
	defaultNameLanguage: string;
	deleted?: boolean;
	description: EnglishTranslatedStringContract;
	discs: AlbumDiscPropertiesContract[];
	discType: AlbumType;
	id: number;
	identifiers: string[];
	name?: string;
	names: LocalizedStringWithIdContract[];
	originalRelease: AlbumReleaseContract;
	pictures: EntryPictureFileContract[];
	pvs: PVContract[];
	songs: SongInAlbumEditContract[];
	status: EntryStatus;
	updateNotes?: string;
	webLinks: WebLinkContract[];
}
