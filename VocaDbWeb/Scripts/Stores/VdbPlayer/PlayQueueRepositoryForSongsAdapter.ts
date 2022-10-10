import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVService } from '@/Models/PVs/PVService';
import {
	SongGetListQueryParams,
	SongRepository,
} from '@/Repositories/SongRepository';
import {
	EntryType,
	PlayQueueRepository,
	PlayQueueSongContract,
} from '@/Stores/VdbPlayer/PlayQueueRepository';

export class PlayQueueRepositoryForSongsAdapter
	implements PlayQueueRepository<SongGetListQueryParams> {
	public constructor(private readonly songRepo: SongRepository) {}

	public getSongs = async ({
		lang,
		pagingProps,
		pvServices,
		queryParams,
	}: {
		lang: ContentLanguagePreference;
		pagingProps: PagingProperties;
		pvServices?: PVService[];
		queryParams: SongGetListQueryParams;
	}): Promise<PartialFindResultContract<PlayQueueSongContract>> => {
		const { items, totalCount } = await this.songRepo.getList({
			fields: PlayQueueRepository.songOptionalFields,
			lang: lang,
			paging: pagingProps,
			pvServices: pvServices,
			queryParams: queryParams,
		});

		const songs = items.map(
			(song): PlayQueueSongContract => ({
				entryType: EntryType.Song,
				id: song.id,
				name: song.name,
				status: song.status,
				additionalNames: song.additionalNames,
				urlThumb: song.mainPicture?.urlThumb ?? '',
				pvs: song.pvs ?? [],
				artistIds:
					song.artists
						?.filter(({ artist }) => artist !== undefined)
						.map((artist) => artist.artist!.id) ?? [],
				tagIds: song.tags?.map((tag) => tag.tag.id) ?? [],
				artistString: song.artistString,
				songType: song.songType,
			}),
		);

		return { items: songs, totalCount: totalCount };
	};
}
