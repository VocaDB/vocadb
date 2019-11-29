
import CommonEntryContract from '../CommonEntryContract';
import EntryThumbContract from '../EntryThumbContract';
import EntryWithTagUsagesContract from '../Base/EntryWithTagUsagesContract';

//module vdb.dataContracts {

	export interface ArtistApiContract extends CommonEntryContract, EntryWithTagUsagesContract {

		additionalNames: string;

		artistType: string;

		mainPicture: EntryThumbContract;

	}

//} 