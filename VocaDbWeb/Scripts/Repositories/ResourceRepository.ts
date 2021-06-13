import ResourcesContract from '@DataContracts/ResourcesContract';
import HttpClient from '@Shared/HttpClient';

import { mergeUrls } from './BaseRepository';
import RepositoryParams from './RepositoryParams';

export default class ResourceRepository {
	public constructor(private readonly httpClient: HttpClient) {}

	public getList = ({
		baseUrl,
		cultureCode,
		setNames,
	}: RepositoryParams & {
		cultureCode: string;
		setNames: string[];
	}): Promise<ResourcesContract> => {
		var url = mergeUrls(baseUrl, `/api/resources/${cultureCode}/`);
		return this.httpClient.get<ResourcesContract>(url, { setNames: setNames });
	};
}
