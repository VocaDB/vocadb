import { EntryWithTagUsagesContract } from '@/DataContracts/Base/EntryWithTagUsagesContract';
import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { EntryStatus } from '@/Models/EntryStatus';
import { EntryType } from '@/Models/EntryType';
import { SongType } from '@/Models/Songs/SongType';

// Base data contract for entries from the API.
// Corresponds to C# datacontract EntryForApiContract.
export interface EntryContract extends EntryWithTagUsagesContract {
	additionalNames?: string;
	artistString?: string;
	artistType?: ArtistType;
	discType?: AlbumType;
	entryType: EntryType;
	eventCategory?: string;
	id: number;
	mainPicture?: EntryThumbContract;
	name: string;
	releaseEventSeriesName?: string;
	songListFeaturedCategory?: string;
	songType?: SongType;
	status: EntryStatus;
	tagCategoryName?: string;
	urlSlug?: string;
}
