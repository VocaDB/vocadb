import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVService } from '@/Models/PVs/PVService';
import {
	SongListGetSongsQueryParams,
	SongListRepository,
} from '@/Repositories/SongListRepository';
import { PlayQueueRepository } from '@/Stores/VdbPlayer/PlayQueueRepository';
import {
	EntryStatus,
	EntryType,
	PlayQueueSongContract,
	PVType,
} from '@/Stores/VdbPlayer/PlayQueueRepository';

export class PlayQueueRepositoryForSongListAdapter
	implements PlayQueueRepository<SongListGetSongsQueryParams> {
	public constructor(private readonly songListRepo: SongListRepository) {}

	public getSongs = async ({
		lang,
		pagingProps,
		pvServices,
		queryParams,
	}: {
		lang: ContentLanguagePreference;
		pagingProps: PagingProperties;
		pvServices?: PVService[];
		queryParams: SongListGetSongsQueryParams;
	}): Promise<PartialFindResultContract<PlayQueueSongContract>> => {
		const { items, totalCount } = await this.songListRepo.getSongs({
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
				status: song.status as EntryStatus /* TODO: enum */,
				additionalNames: song.additionalNames,
				urlThumb: song.mainPicture?.urlThumb ?? '',
				pvs:
					song.pvs?.map((pv) => ({
						id: pv.id ?? 0,
						service: pv.service,
						pvId: pv.pvId,
						pvType: pv.pvType as PVType /* TODO: enum */,
					})) ?? [],
				artistString: song.artistString,
				songType: song.songType,
			}));

		return { items: songs, totalCount: totalCount };
	};
}
