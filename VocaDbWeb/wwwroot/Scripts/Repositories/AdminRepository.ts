import WebhookContract from '@DataContracts/WebhookContract';
import AjaxHelper from '@Helpers/AjaxHelper';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import { IPRuleContract } from '@ViewModels/Admin/ManageIPRulesViewModel';

export default class AdminRepository {
  constructor(
    private readonly httpClient: HttpClient,
    private urlMapper: UrlMapper,
  ) {}

  public addIpToBanList = (rule: IPRuleContract): Promise<boolean> => {
    return this.httpClient.post<boolean>(
      this.urlMapper.mapRelative('/api/ip-rules'),
      rule,
    );
  };

  public checkSFS = (
    ip: string,
    callback: (html: string) => void,
  ): JQueryXHR => {
    return $.get(
      this.urlMapper.mapRelative('/Admin/CheckSFS'),
      { ip: ip },
      callback,
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

  public saveWebhooks = (webhooks: WebhookContract[]): Promise<any> => {
    var url = this.urlMapper.mapRelative('/api/webhooks');
    return Promise.resolve(AjaxHelper.putJSON(url, webhooks));
  };
}
