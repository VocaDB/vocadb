import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { PlayQueueHelper } from '@/Helpers/PlayQueueHelper';
import { PVService } from '@/Models/PVs/PVService';
import {
	SongListGetSongsQueryParams,
	SongListRepository,
} from '@/Repositories/SongListRepository';
import { PlayQueueRepository } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';

export class PlayQueueRepositoryForSongListAdapter
	implements PlayQueueRepository<SongListGetSongsQueryParams> {
	public constructor(private readonly songListRepo: SongListRepository) {}

	public getItems = async (
		pvServices: PVService[],
		pagingProps: PagingProperties,
		queryParams: SongListGetSongsQueryParams,
	): Promise<PartialFindResultContract<PlayQueueItem>> => {
		const { items, totalCount } = await this.songListRepo.getSongsWithPVs({
			lang: vdb.values.languagePreference,
			paging: pagingProps,
			pvServices: pvServices,
			queryParams: queryParams,
		});

		const playQueueItems = PlayQueueHelper.createItemsFromSongs(
			items.map((songInList) => songInList.song),
		);

		return { items: playQueueItems, totalCount: totalCount };
	};
}
