
import ArchivedVersionContract from '../Versioning/ArchivedVersionContract';
import EntryContract from '../EntryContract';
import UserApiContract from '../User/UserApiContract';

//module vdb.dataContracts.activityEntry {
	
	export interface ActivityEntryContract {
		
		archivedVersion: ArchivedVersionContract;

		author: UserApiContract;

		createDate: string;

		editEvent: string;

		entry: EntryContract;

	}

//} 