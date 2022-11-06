import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { RatedSongForUserForApiContract } from '@/DataContracts/User/RatedSongForUserForApiContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVService } from '@/Models/PVs/PVService';
import { SongOptionalField } from '@/Repositories/SongRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { AdvancedSearchFilters } from '@/Stores/Search/AdvancedSearchFilters';
import { ArtistFilters } from '@/Stores/Search/ArtistFilters';
import { SongVoteRating } from '@/Stores/Search/SongSearchStore';
import {
	IPlayListRepository,
	ISongForPlayList,
} from '@/Stores/Song/PlayList/PlayListStore';
import { RatedSongForUserSortRule } from '@/Stores/User/RatedSongsSearchStore';

export interface IRatedSongsAdapterStore {
	userId: number;
	searchTerm: string;
	tagIds: number[];
	artistFilters: ArtistFilters;
	rating: SongVoteRating;
	songListId?: number;
	advancedFilters: AdvancedSearchFilters;
	groupByRating: boolean;
	sort: RatedSongForUserSortRule;
}

export class PlayListRepositoryForRatedSongsAdapter
	implements IPlayListRepository {
	constructor(
		private readonly userRepo: UserRepository,
		private readonly ratedSongsAdapterStore: IRatedSongsAdapterStore,
	) {}

	getSongs(
		pvServices: PVService[],
		paging: PagingProperties,
		fields: SongOptionalField[],
		lang: ContentLanguagePreference,
	): Promise<PartialFindResultContract<ISongForPlayList>> {
		return this.userRepo
			.getRatedSongsList({
				fields: [SongOptionalField.MainPicture],
				lang: lang,
				paging: paging,
				pvServices: pvServices,
				queryParams: {
					userId: this.ratedSongsAdapterStore.userId,
					query: this.ratedSongsAdapterStore.searchTerm,
					tagIds: this.ratedSongsAdapterStore.tagIds,
					artistIds: this.ratedSongsAdapterStore.artistFilters.artistIds,
					childVoicebanks: this.ratedSongsAdapterStore.artistFilters
						.childVoicebanks,
					rating: this.ratedSongsAdapterStore.rating,
					songListId: this.ratedSongsAdapterStore.songListId,
					advancedFilters: this.ratedSongsAdapterStore.advancedFilters.filters,
					groupByRating: this.ratedSongsAdapterStore.groupByRating,
					sort: this.ratedSongsAdapterStore.sort,
				},
			})
			.then(
				(result: PartialFindResultContract<RatedSongForUserForApiContract>) => {
					const mapped = result.items.map((song, index) => ({
						name: song.song!.name,
						song: song.song!,
						indexInPlayList: paging.start! + index,
					}));

					return { items: mapped, totalCount: result.totalCount };
				},
			);
	}
}
