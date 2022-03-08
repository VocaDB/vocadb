import UserApiContract from '../User/UserApiContract';

// C# class: ArchivedObjectVersionForApiContract
export default interface ArchivedVersionContract {
	agentName: string;
	author?: UserApiContract;
	changedFields: string[];
	created: Date;
	editEvent: string;
	hidden: boolean;
	id: number;
	notes: string;
	status: string;
	version: number;
}
