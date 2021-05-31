import HttpClient from '@Shared/HttpClient';

export default class EntryReportRepository {
	constructor(private readonly httpClient: HttpClient) {}

	public getNewReportCount = (): Promise<number> => {
		return this.httpClient.get<number>('/entryReports/newReportsCount');
	};
}
