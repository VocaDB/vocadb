import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';

export interface PlayQueueRepository<TQueryParams> {
	getItems(
		pagingProperties: PagingProperties,
		queryParams: TQueryParams,
	): Promise<PartialFindResultContract<PlayQueueItem>>;
}
