import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVService } from '@/Models/PVs/PVService';
import { SongType } from '@/Models/Songs/SongType';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { SongOptionalField } from '@/Repositories/SongRepository';
import { AdvancedSearchFilters } from '@/Stores/Search/AdvancedSearchFilters';
import { ArtistFilters } from '@/Stores/Search/ArtistFilters';
import {
	IPlayListRepository,
	ISongForPlayList,
} from '@/Stores/Song/PlayList/PlayListStore';

export interface ISongListStore {
	query: string;
	songType: SongType;
	tagIds: number[];
	childTags: boolean;
	artistFilters: ArtistFilters;
	advancedFilters: AdvancedSearchFilters;
	sort: string /* TODO: enum */;
}

export class PlayListRepositoryForSongListAdapter
	implements IPlayListRepository {
	constructor(
		private readonly songListRepo: SongListRepository,
		private readonly songListId: number,
		private readonly songListStore: ISongListStore,
	) {}

	getSongs = (
		pvServices: PVService[],
		paging: PagingProperties,
		fields: SongOptionalField[],
		lang: ContentLanguagePreference,
	): Promise<PartialFindResultContract<ISongForPlayList>> =>
		this.songListRepo
			.getSongs({
				fields: fields,
				lang: lang,
				paging: paging,
				pvServices: pvServices,
				queryParams: {
					listId: this.songListId,
					query: this.songListStore.query,
					songTypes:
						this.songListStore.songType !== SongType.Unspecified
							? [this.songListStore.songType]
							: undefined,
					tagIds: this.songListStore.tagIds,
					childTags: this.songListStore.childTags,
					artistIds: this.songListStore.artistFilters.artistIds,
					artistParticipationStatus: this.songListStore.artistFilters
						.artistParticipationStatus,
					childVoicebanks: this.songListStore.artistFilters.childVoicebanks,
					advancedFilters: this.songListStore.advancedFilters.filters,
					sort: this.songListStore.sort,
				},
			})
			.then((result) => {
				const mapped = result.items.map((song, idx) => ({
					name: `${song.order}. ${song.song.name}${
						song.notes ? ` (${song.notes})` : ''
					}`,
					song: song.song,
					indexInPlayList: paging.start! + idx,
				}));

				return { items: mapped, totalCount: result.totalCount };
			});
}
