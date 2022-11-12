import { PVContract } from '@/DataContracts/PVs/PVContract';
import { PVType } from '@/Models/PVs/PVType';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';

export class PVRepository {
	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	getPVByUrl = ({
		pvUrl,
		type,
	}: {
		pvUrl: string;
		type: PVType;
	}): Promise<PVContract> => {
		var url = this.urlMapper.mapRelative('/api/pvs');
		return this.httpClient.get<PVContract>(url, { pvUrl: pvUrl, type: type });
	};
}
