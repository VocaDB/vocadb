import { EntryContract } from '@/DataContracts/EntryContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { ArchivedVersionContract } from '@/DataContracts/Versioning/ArchivedVersionContract';
import { EntryEditEvent } from '@/Models/ActivityEntries/EntryEditEvent';

export interface ActivityEntryContract {
	archivedVersion?: ArchivedVersionContract;
	author: UserApiContract;
	createDate: string;
	editEvent: EntryEditEvent;
	entry: EntryContract;
}
