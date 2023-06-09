import { UserApiContract } from '@/types/DataContracts/User/UserApiContract';

export interface AuditLogEntryContract {
	action: string;
	agentName: string;
	id: number;
	time: string;
	user?: UserApiContract;
}
