import { EntryContract } from './EntryContract';
import { UserApiContract } from './User/UserApiContract';
import { ArchivedVersionContract } from './Versioning/ArchivedVersionContract';

export enum ReportStatus {
	Open = 'Open',
	Closed = 'Closed',
}

// C# class: EntryReportForApiContract
export interface EntryReportContract {
	closedBy?: UserApiContract;
	closedAt?: string;
	created: string;
	entry: EntryContract;
	hostname: string;
	id: number;
	notes: string;
	reportTypeName: string;
	user?: UserApiContract;
	version?: ArchivedVersionContract;
}
