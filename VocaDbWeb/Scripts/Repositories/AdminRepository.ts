import WebhookContract from '@DataContracts/WebhookContract';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import { IPRuleContract } from '@ViewModels/Admin/ManageIPRulesViewModel';

import RepositoryParams from './RepositoryParams';

export default class AdminRepository {
	public constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	public addIpToBanList = ({
		baseUrl,
		rule,
	}: RepositoryParams & {
		rule: IPRuleContract;
	}): Promise<boolean> => {
		return this.httpClient.post<boolean>(
			this.urlMapper.mapRelative('/api/ip-rules'),
			rule,
		);
	};

	public checkSFS = ({
		baseUrl,
		ip,
	}: RepositoryParams & {
		ip: string;
	}): Promise<string> => {
		return this.httpClient.get<string>(
			this.urlMapper.mapRelative('/Admin/CheckSFS'),
			{ ip: ip },
		);
	};

	public getTempBannedIps = ({
		baseUrl,
	}: RepositoryParams & {}): Promise<string[]> => {
		return this.httpClient.get<string[]>(
			this.urlMapper.mapRelative('/api/admin/tempBannedIPs'),
		);
	};

	public getWebhooks = ({
		baseUrl,
	}: RepositoryParams & {}): Promise<WebhookContract[]> => {
		return this.httpClient.get<WebhookContract[]>(
			this.urlMapper.mapRelative('/api/webhooks'),
		);
	};

	public saveWebhooks = ({
		baseUrl,
		webhooks,
	}: RepositoryParams & {
		webhooks: WebhookContract[];
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative('/api/webhooks');
		return this.httpClient.put<void>(url, webhooks);
	};
}
