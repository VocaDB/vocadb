
import TagUsageForApiContract from '../Tag/TagUsageForApiContract';

//module vdb.dataContracts {
	
	export default interface EntryWithTagUsagesContract {
		
		id: number;

		name: string;

		tags?: TagUsageForApiContract[];

	}

//}