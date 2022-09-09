import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { PlayQueueHelper } from '@/Helpers/PlayQueueHelper';
import { SongRepository } from '@/Repositories/SongRepository';
import { PlayQueueRepository } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';

export class PlayQueueRepositoryForSongsAdapter
	implements
		PlayQueueRepository<
			Parameters<SongRepository['getList']>[0]['queryParams']
		> {
	public constructor(private readonly songRepo: SongRepository) {}

	public getItems = async (
		pagingProperties: PagingProperties,
		queryParams: Parameters<SongRepository['getList']>[0]['queryParams'],
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
