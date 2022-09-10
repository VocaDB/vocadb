import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { PlayQueueHelper } from '@/Helpers/PlayQueueHelper';
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
		pagingProperties: PagingProperties,
		queryParams: SongGetListQueryParams,
	): Promise<PartialFindResultContract<PlayQueueItem>> => {
		const songs = await this.songRepo.getListWithPVs({
			lang: vdb.values.languagePreference,
			paging: pagingProperties,
			queryParams: queryParams,
		});

		const items = PlayQueueHelper.createItemsFromSongs(songs.items);

		return { items: items, totalCount: songs.totalCount };
	};
}
