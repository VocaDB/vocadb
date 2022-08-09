import { AlbumDiscPropertiesContract } from '@/DataContracts/Album/AlbumDiscPropertiesContract';
import { AlbumReleaseContract } from '@/DataContracts/Album/AlbumReleaseContract';
import { ArtistForAlbumContract } from '@/DataContracts/ArtistForAlbumContract';
import { EntryPictureFileContract } from '@/DataContracts/EntryPictureFileContract';
import { EnglishTranslatedStringContract } from '@/DataContracts/Globalization/EnglishTranslatedStringContract';
import { LocalizedStringWithIdContract } from '@/DataContracts/Globalization/LocalizedStringWithIdContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { SongInAlbumEditContract } from '@/DataContracts/Song/SongInAlbumEditContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { AlbumType } from '@/Models/Albums/AlbumType';

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
	status: string;
	updateNotes?: string;
	webLinks: WebLinkContract[];
}
