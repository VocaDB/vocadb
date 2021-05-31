import WebhookContract from '@DataContracts/WebhookContract';
import HttpClient from '@Shared/HttpClient';
import { IPRuleContract } from '@ViewModels/Admin/ManageIPRulesViewModel';

export default class AdminRepository {
	constructor(private readonly httpClient: HttpClient) {}

	public addIpToBanList = (rule: IPRuleContract): Promise<boolean> => {
		return this.httpClient.post<boolean>('/api/ip-rules', rule);
	};

	public checkSFS = (ip: string): Promise<string> => {
		return this.httpClient.get<string>('/Admin/CheckSFS', { ip: ip });
	};

	public getTempBannedIps = (): Promise<string[]> => {
		return this.httpClient.get<string[]>('/api/admin/tempBannedIPs');
	};

	public getWebhooks = (): Promise<WebhookContract[]> => {
		return this.httpClient.get<WebhookContract[]>('/api/webhooks');
	};

	public saveWebhooks = (webhooks: WebhookContract[]): Promise<void> => {
		return this.httpClient.put<void>('/api/webhooks', webhooks);
	};
}
