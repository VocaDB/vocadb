import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { PlayQueueHelper } from '@/Helpers/PlayQueueHelper';
import { PVService } from '@/Models/PVs/PVService';
import {
	SongGetListQueryParams,
	SongRepository,
} from '@/Repositories/SongRepository';
import { PlayQueueRepository } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';

export class PlayQueueRepositoryForSongsAdapter
	implements PlayQueueRepository<SongGetListQueryParams> {
	public constructor(private readonly songRepo: SongRepository) {}

	public getItems = async (
		pvServices: PVService[],
		pagingProps: PagingProperties,
		queryParams: SongGetListQueryParams,
	): Promise<PartialFindResultContract<PlayQueueItem>> => {
		const { items, totalCount } = await this.songRepo.getListWithPVs({
			lang: vdb.values.languagePreference,
			paging: pagingProps,
			pvServices: pvServices,
			queryParams: queryParams,
		});

		const playQueueItems = PlayQueueHelper.createItemsFromSongs(items);

		return { items: playQueueItems, totalCount: totalCount };
	};
}
