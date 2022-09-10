import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { PlayQueueHelper } from '@/Helpers/PlayQueueHelper';
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
		pagingProperties: PagingProperties,
		queryParams: SongListGetSongsQueryParams,
	): Promise<PartialFindResultContract<PlayQueueItem>> => {
		const songsInList = await this.songListRepo.getSongsWithPVs({
			lang: vdb.values.languagePreference,
			paging: pagingProperties,
			queryParams: queryParams,
		});

		const items = PlayQueueHelper.createItemsFromSongs(
			songsInList.items.map((songInList) => songInList.song),
		);

		return { items: items, totalCount: songsInList.totalCount };
	};
}
