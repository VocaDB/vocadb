import { injectable } from 'inversify';
import 'reflect-metadata';

import HttpClient from './HttpClient';

@injectable()
export default class HttpClientFactory {
	public createClient = (baseUrl?: string): HttpClient => {
		const httpClient = new HttpClient();
		httpClient.baseUrl = baseUrl;
		return httpClient;
	};
}
