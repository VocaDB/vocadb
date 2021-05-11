import { IPRuleContract } from '@ViewModels/Admin/ManageIPRulesViewModel';
import UrlMapper from '@Shared/UrlMapper';
import HttpClient from '@Shared/HttpClient';

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
}
