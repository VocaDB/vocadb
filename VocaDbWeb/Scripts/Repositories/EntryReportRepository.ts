import HttpClient from '@Shared/HttpClient';

import { mergeUrls } from './BaseRepository';
import RepositoryParams from './RepositoryParams';

export default class EntryReportRepository {
	public constructor(private readonly httpClient: HttpClient) {}

	public getNewReportCount = ({
		baseUrl,
	}: RepositoryParams & {}): Promise<number> => {
		var url = mergeUrls(baseUrl, '/entryReports/newReportsCount');
		return this.httpClient.get<number>(url);
	};
}
