import EntryThumbContract from './EntryThumbContract';
import EntryWithTagUsagesContract from './Base/EntryWithTagUsagesContract';

//module vdb.dataContracts {
	
	// Base data contract for entries from the API.
	// Corresponds to C# datacontract EntryForApiContract.
	export default interface EntryContract extends EntryWithTagUsagesContract {

		additionalNames: string;

		artistType?: string;

		discType?: string;

		entryType: string;

		eventCategory?: string;

		id: number;

		mainPicture: EntryThumbContract;

		name: string;

		releaseEventSeriesName?: string;

		songListFeaturedCategory?: string;

		songType?: string;

		tagCategoryName?: string;

		urlSlug?: string;

	}

//}