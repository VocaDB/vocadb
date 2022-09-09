import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { PlayQueueHelper } from '@/Helpers/PlayQueueHelper';
import { UserRepository } from '@/Repositories/UserRepository';
import { PlayQueueRepository } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';

export class PlayQueueRepositoryForRatedSongsAdapter
	implements
		PlayQueueRepository<
			Parameters<UserRepository['getRatedSongsList']>[0]['queryParams']
		> {
	public constructor(private readonly userRepo: UserRepository) {}

	public getItems = async (
		pagingProperties: PagingProperties,
		queryParams: Parameters<
			UserRepository['getRatedSongsList']
		>[0]['queryParams'],
	): Promise<PartialFindResultContract<PlayQueueItem>> => {
		const songsForUser = await this.userRepo.getRatedSongsListWithPVs({
			lang: vdb.values.languagePreference,
			paging: pagingProperties,
			queryParams: queryParams,
		});

		const items = PlayQueueHelper.createItemsFromSongs(
			songsForUser.items.map((songForUser) => songForUser.song),
		);

		return {
			items: items,
			totalCount: songsForUser.totalCount,
		};
	};
}
