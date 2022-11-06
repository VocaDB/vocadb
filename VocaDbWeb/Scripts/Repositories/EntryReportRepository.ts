import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';

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
