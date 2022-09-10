import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { PVService } from '@/Models/PVs/PVService';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';

export interface PlayQueueRepository<TQueryParams> {
	getItems(
		pvServices: PVService[],
		pagingProperties: PagingProperties,
		queryParams: TQueryParams,
	): Promise<PartialFindResultContract<PlayQueueItem>>;
}
