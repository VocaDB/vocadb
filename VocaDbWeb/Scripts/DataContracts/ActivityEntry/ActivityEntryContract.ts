import EntryContract from '../EntryContract';
import UserApiContract from '../User/UserApiContract';
import ArchivedVersionContract from '../Versioning/ArchivedVersionContract';

export default interface ActivityEntryContract {
	archivedVersion: ArchivedVersionContract;

	author: UserApiContract;

	createDate: string;

	editEvent: string;

	entry: EntryContract;
}
