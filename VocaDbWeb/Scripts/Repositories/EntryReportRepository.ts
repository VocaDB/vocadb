import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

export default class EntryReportRepository {
  constructor(
    private readonly httpClient: HttpClient,
    private urlMapper: UrlMapper,
  ) {}

  public getNewReportCount = (): Promise<number> => {
    var url = this.urlMapper.mapRelative('/entryReports/newReportsCount');
    return this.httpClient.get<number>(url);
  };
}
