import { EntryWithTagUsagesContract } from '@/DataContracts/Base/EntryWithTagUsagesContract';
import { EntryThumbContract } from '@/DataContracts/EntryThumbContract';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { SongType } from '@/Models/Songs/SongType';

// Base data contract for entries from the API.
// Corresponds to C# datacontract EntryForApiContract.
export interface EntryContract extends EntryWithTagUsagesContract {
	additionalNames?: string;

	artistString?: string;

	artistType?: ArtistType;

	discType?: AlbumType;

	entryType: string;

	eventCategory?: string;

	id: number;

	mainPicture?: EntryThumbContract;

	name: string;

	releaseEventSeriesName?: string;

	songListFeaturedCategory?: string;

	songType?: SongType;

	status?: string;

	tagCategoryName?: string;

	urlSlug?: string;
}
