import EntryContract from '@/DataContracts/EntryContract';
import UserApiContract from '@/DataContracts/User/UserApiContract';
import ArchivedVersionContract from '@/DataContracts/Versioning/ArchivedVersionContract';

export default interface ActivityEntryContract {
	archivedVersion: ArchivedVersionContract;

	author: UserApiContract;

	createDate: string;

	editEvent: string;

	entry: EntryContract;
}
