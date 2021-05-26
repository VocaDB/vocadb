import WebhookContract from '@DataContracts/WebhookContract';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import { IPRuleContract } from '@ViewModels/Admin/ManageIPRulesViewModel';

export default class AdminRepository {
  constructor(
    private readonly httpClient: HttpClient,
    private readonly urlMapper: UrlMapper,
  ) {}

  public addIpToBanList = (rule: IPRuleContract): Promise<boolean> => {
    return this.httpClient.post<boolean>(
      this.urlMapper.mapRelative('/api/ip-rules'),
      rule,
    );
  };

  public checkSFS = (ip: string): Promise<string> => {
    return this.httpClient.get<string>(
      this.urlMapper.mapRelative('/Admin/CheckSFS'),
      { ip: ip },
    );
  };

  public getTempBannedIps = (): Promise<string[]> => {
    return this.httpClient.get<string[]>(
      this.urlMapper.mapRelative('/api/admin/tempBannedIPs'),
    );
  };

  public getWebhooks = (): Promise<WebhookContract[]> => {
    return this.httpClient.get<WebhookContract[]>(
      this.urlMapper.mapRelative('/api/webhooks'),
    );
  };

  public saveWebhooks = (webhooks: WebhookContract[]): Promise<void> => {
    var url = this.urlMapper.mapRelative('/api/webhooks');
    return this.httpClient.put<void>(url, webhooks);
  };
}
