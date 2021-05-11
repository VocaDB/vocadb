import UrlMapper from '@Shared/UrlMapper';
import HttpClient from '@Shared/HttpClient';

export default class EntryReportRepository {
  public getNewReportCount = (): Promise<number> => {
    var url = this.urlMapper.mapRelative('/entryReports/newReportsCount');
    return this.httpClient.get<number>(url);
  };

  constructor(
    private readonly httpClient: HttpClient,
    private urlMapper: UrlMapper,
  ) {}
}
