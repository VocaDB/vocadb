import { AuditLogEntryContract } from '@/DataContracts/AuditLogEntryContract';
import { WebhookContract } from '@/DataContracts/WebhookContract';
import { UserGroup } from '@/Models/Users/UserGroup';
import { httpClient, HttpClient } from '@/Shared/HttpClient';
import { urlMapper, UrlMapper } from '@/Shared/UrlMapper';

export interface IPRuleContract {
	address?: string;
	created?: string;
	id?: number;
	notes?: string;
}

export class AdminRepository {
	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	// eslint-disable-next-line no-empty-pattern
	getIPRules = ({}: {}): Promise<IPRuleContract[]> => {
		return this.httpClient.get<IPRuleContract[]>(
			this.urlMapper.mapRelative('/api/ip-rules'),
		);
	};

	saveIPRules = ({ ipRules }: { ipRules: IPRuleContract[] }): Promise<void> => {
		return this.httpClient.put<void>(
			this.urlMapper.mapRelative('/api/ip-rules'),
			ipRules,
		);
	};

	addIpToBanList = ({ rule }: { rule: IPRuleContract }): Promise<boolean> => {
		return this.httpClient.post<boolean>(
			this.urlMapper.mapRelative('/api/ip-rules'),
			rule,
		);
	};

	checkSFS = ({ ip }: { ip: string }): Promise<string> => {
		return this.httpClient.get<string>(
			this.urlMapper.mapRelative('/Admin/CheckSFS'),
			{ ip: ip },
		);
	};

	// eslint-disable-next-line no-empty-pattern
	getTempBannedIps = ({}: {}): Promise<string[]> => {
		return this.httpClient.get<string[]>(
			this.urlMapper.mapRelative('/api/admin/tempBannedIPs'),
		);
	};

	// eslint-disable-next-line no-empty-pattern
	getWebhooks = ({}: {}): Promise<WebhookContract[]> => {
		return this.httpClient.get<WebhookContract[]>(
			this.urlMapper.mapRelative('/api/webhooks'),
		);
	};

	saveWebhooks = ({
		webhooks,
	}: {
		webhooks: WebhookContract[];
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative('/api/webhooks');
		return this.httpClient.put<void>(url, webhooks);
	};

	getAuditLogEntries = ({
		excludeUsers,
		filter,
		groupId,
		onlyNewUsers,
		userName,
		start,
	}: {
		excludeUsers: string;
		filter: string;
		groupId?: UserGroup;
		onlyNewUsers: boolean;
		userName: string;
		start: number;
	}): Promise<AuditLogEntryContract[]> => {
		return this.httpClient.get<AuditLogEntryContract[]>(
			this.urlMapper.mapRelative('/api/admin/audit-logs'),
			{
				excludeUsers: excludeUsers,
				filter: filter,
				groupId: groupId,
				onlyNewUsers: onlyNewUsers,
				userName: userName,
				start: start,
			},
		);
	};

	deleteEntryReport = (
		requestToken: string,
		{ id }: { id: number },
	): Promise<void> => {
		return this.httpClient.delete(
			this.urlMapper.mapRelative(`/api/admin/reports/${id}`),
		);
	};
}

export const adminRepo = new AdminRepository(httpClient, urlMapper);
