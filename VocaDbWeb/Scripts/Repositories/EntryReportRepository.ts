import { httpClient, HttpClient } from '@/Shared/HttpClient';
import { urlMapper, UrlMapper } from '@/Shared/UrlMapper';

export class EntryReportRepository {
	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	// eslint-disable-next-line no-empty-pattern
	getNewReportCount = ({}: {}): Promise<number> => {
		var url = this.urlMapper.mapRelative('/entryReports/newReportsCount');
		return this.httpClient.get<number>(url);
	};
}

export const entryReportRepo = new EntryReportRepository(httpClient, urlMapper);
