import { UserApiContract } from '@/DataContracts/User/UserApiContract';

export interface AuditLogEntryContract {
	action: string;
	agentName: string;
	id: number;
	/** DateTime */
	time: string;
	user?: UserApiContract;
}
