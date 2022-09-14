import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { PlayQueueHelper } from '@/Helpers/PlayQueueHelper';
import { PVService } from '@/Models/PVs/PVService';
import {
	UserGetRatedSongsListQueryParams,
	UserRepository,
} from '@/Repositories/UserRepository';
import { PlayQueueRepository } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueItem } from '@/Stores/VdbPlayer/PlayQueueStore';

export class PlayQueueRepositoryForRatedSongsAdapter
	implements PlayQueueRepository<UserGetRatedSongsListQueryParams> {
	public constructor(private readonly userRepo: UserRepository) {}

	public getItems = async (
		pvServices: PVService[],
		pagingProps: PagingProperties,
		queryParams: UserGetRatedSongsListQueryParams,
	): Promise<PartialFindResultContract<PlayQueueItem>> => {
		const { items, totalCount } = await this.userRepo.getRatedSongsListWithPVs({
			lang: vdb.values.languagePreference,
			paging: pagingProps,
			pvServices: pvServices,
			queryParams: queryParams,
		});

		const playQueueItems = PlayQueueHelper.createItemsFromSongs(
			items.map((songForUser) => songForUser.song),
		);

		return { items: playQueueItems, totalCount: totalCount };
	};
}
