import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVService } from '@/Models/PVs/PVService';
import {
	UserGetRatedSongsListQueryParams,
	UserRepository,
} from '@/Repositories/UserRepository';
import { PlayQueueRepository } from '@/Stores/VdbPlayer/PlayQueueRepository';
import {
	EntryType,
	PlayQueueSongContract,
} from '@/Stores/VdbPlayer/PlayQueueRepository';

export class PlayQueueRepositoryForRatedSongsAdapter
	implements PlayQueueRepository<UserGetRatedSongsListQueryParams> {
	public constructor(private readonly userRepo: UserRepository) {}

	public getSongs = async ({
		lang,
		pagingProps,
		pvServices,
		queryParams,
	}: {
		lang: ContentLanguagePreference;
		pagingProps: PagingProperties;
		pvServices?: PVService[];
		queryParams: UserGetRatedSongsListQueryParams;
	}): Promise<PartialFindResultContract<PlayQueueSongContract>> => {
		const { items, totalCount } = await this.userRepo.getRatedSongsList({
			fields: PlayQueueRepository.songOptionalFields,
			lang: lang,
			paging: pagingProps,
			pvServices: pvServices,
			queryParams: queryParams,
		});

		const songs = items
			.map(({ song }) => song)
			.filter((song) => !!song)
			.map((song) => song!)
			.map((song) => ({
				entryType: EntryType.Song as const /* TODO: enum */,
				id: song.id,
				name: song.name,
				status: song.status,
				additionalNames: song.additionalNames,
				urlThumb: song.mainPicture?.urlThumb ?? '',
				pvs: song.pvs ?? [],
				artistString: song.artistString,
				songType: song.songType,
			}));

		return { items: songs, totalCount: totalCount };
	};
}
