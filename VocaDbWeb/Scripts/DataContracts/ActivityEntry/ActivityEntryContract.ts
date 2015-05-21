
module vdb.dataContracts.activityEntry {
	
	export interface ActivityEntryContract {
		
		archivedVersion: versioning.ArchivedVersionContract;

		author: user.UserApiContract;

		createDate: string;

		editEvent: string;

		entry: EntryContract;

	}

} 