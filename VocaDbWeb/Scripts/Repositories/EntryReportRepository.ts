import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

export default class EntryReportRepository {
	public constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	// eslint-disable-next-line no-empty-pattern
	public getNewReportCount = ({}: {}): Promise<number> => {
		var url = this.urlMapper.mapRelative('/entryReports/newReportsCount');
		return this.httpClient.get<number>(url);
	};
}
