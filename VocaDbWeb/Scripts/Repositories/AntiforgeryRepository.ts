import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';

export class AntiforgeryRepository {
	public constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	public getToken = async (): Promise<string> => {
		await this.httpClient.get<void>(
			this.urlMapper.mapRelative('/api/antiforgery/token'),
		);

		// Code from: https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-6.0#javascript-1.
		// https://developer.mozilla.org/docs/web/api/document/cookie
		const xsrfToken = document.cookie
			.split('; ')
			.find((row) => row.startsWith('XSRF-TOKEN='))!
			.split('=')[1];

		return xsrfToken;
	};
}
