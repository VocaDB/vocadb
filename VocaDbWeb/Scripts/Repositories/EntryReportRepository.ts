import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

import RepositoryParams from './RepositoryParams';

export default class EntryReportRepository {
	public constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	public getNewReportCount = ({
		baseUrl,
	}: RepositoryParams & {}): Promise<number> => {
		var url = this.urlMapper.mapRelative('/entryReports/newReportsCount');
		return this.httpClient.get<number>(url);
	};
}
