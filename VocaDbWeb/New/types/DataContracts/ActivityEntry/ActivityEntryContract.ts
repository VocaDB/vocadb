import { EntryContract } from '@/types/DataContracts/EntryContract';
import { UserApiContract } from '@/types/DataContracts/User/UserApiContract';
import { ArchivedVersionContract } from '@/types/DataContracts/Versioning/ArchivedVersionContract';
import { EntryEditEvent } from '@/types/Models/ActivityEntries/EntryEditEvent';

export interface ActivityEntryContract {
	archivedVersion?: ArchivedVersionContract;
	author?: UserApiContract;
	createDate: string;
	editEvent: EntryEditEvent;
	entry: EntryContract;
}
