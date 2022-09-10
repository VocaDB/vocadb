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
		pagingProperties: PagingProperties,
		queryParams: UserGetRatedSongsListQueryParams,
	): Promise<PartialFindResultContract<PlayQueueItem>> => {
		const songsForUser = await this.userRepo.getRatedSongsListWithPVs({
			lang: vdb.values.languagePreference,
			paging: pagingProperties,
			pvServices: pvServices,
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
