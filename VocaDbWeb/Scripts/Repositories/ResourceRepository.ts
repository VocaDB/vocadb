import { ResourcesContract } from '@/DataContracts/ResourcesContract';
import { functions } from '@/Shared/GlobalFunctions';
import { HttpClient } from '@/Shared/HttpClient';

export class ResourceRepository {
	constructor(
		private readonly httpClient: HttpClient,
		private baseUrl: string,
	) {}

	getList = ({
		cultureCode,
		setNames,
	}: {
		cultureCode: string;
		setNames: string[];
	}): Promise<ResourcesContract> => {
		var url = functions.mergeUrls(
			this.baseUrl,
			`/api/resources/${cultureCode}/`,
		);
		return this.httpClient.get<ResourcesContract>(url, { setNames: setNames });
	};
}
