import { EntryWithTagUsagesContract } from '@/types/DataContracts/Base/EntryWithTagUsagesContract';
import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { AlbumType } from '@/types/Models/Albums/AlbumType';
import { ArtistType } from '@/types/Models/Artists/ArtistType';
import { EntryStatus } from '@/types/Models/EntryStatus';
import { EntryType } from '@/types/Models/EntryType';
import { SongType } from '@/types/Models/Songs/SongType';

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
