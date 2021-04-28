import { IPRuleContract } from '../ViewModels/Admin/ManageIPRulesViewModel';
import UrlMapper from '../Shared/UrlMapper';
import HttpClient from '../Shared/HttpClient';

export default class AdminRepository {
  constructor(
    private readonly httpClient: HttpClient,
    private urlMapper: UrlMapper,
  ) {}

  public addIpToBanList = (
    rule: IPRuleContract,
    callback: (result: boolean) => void,
  ) => {
    return $.postJSON(
      this.urlMapper.mapRelative('/api/ip-rules'),
      rule,
      callback,
    );
  };

  public checkSFS = (ip: string, callback: (html: string) => void) => {
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
