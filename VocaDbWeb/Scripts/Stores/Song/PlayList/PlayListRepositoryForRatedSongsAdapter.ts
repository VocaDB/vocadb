import PagingPropertiesContract from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import RatedSongForUserForApiContract from '@DataContracts/User/RatedSongForUserForApiContract';
import { SongOptionalFields } from '@Models/EntryOptionalFields';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import UserRepository from '@Repositories/UserRepository';
import _ from 'lodash';

import AdvancedSearchFilters from '../../Search/AdvancedSearchFilters';
import ArtistFilters from '../../Search/ArtistFilters';
import { SongVoteRating } from '../../Search/SongSearchStore';
import { RatedSongForUserSortRule } from '../../User/RatedSongsSearchStore';
import { IPlayListRepository, ISongForPlayList } from './PlayListStore';

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

export default class PlayListRepositoryForRatedSongsAdapter
	implements IPlayListRepository {
	public constructor(
		private readonly userRepo: UserRepository,
		private readonly ratedSongsAdapterStore: IRatedSongsAdapterStore,
	) {}

	public getSongs(
		pvServices: string,
		paging: PagingPropertiesContract,
		fields: SongOptionalFields,
		lang: ContentLanguagePreference,
	): Promise<PartialFindResultContract<ISongForPlayList>> {
		return this.userRepo
			.getRatedSongsList({
				userId: this.ratedSongsAdapterStore.userId,
				paging: paging,
				lang: lang,
				query: this.ratedSongsAdapterStore.searchTerm,
				tagIds: this.ratedSongsAdapterStore.tagIds,
				artistIds: this.ratedSongsAdapterStore.artistFilters.artistIds,
				childVoicebanks: this.ratedSongsAdapterStore.artistFilters
					.childVoicebanks,
				rating: this.ratedSongsAdapterStore.rating,
				songListId: this.ratedSongsAdapterStore.songListId,
				advancedFilters: this.ratedSongsAdapterStore.advancedFilters.filters,
				groupByRating: this.ratedSongsAdapterStore.groupByRating,
				pvServices: pvServices,
				fields: 'MainPicture' /* TODO: enum */,
				sort: this.ratedSongsAdapterStore.sort,
			})
			.then(
				(result: PartialFindResultContract<RatedSongForUserForApiContract>) => {
					const mapped = _.map(result.items, (song, index) => {
						return {
							name: song.song!.name,
							song: song.song!,
							indexInPlayList: paging.start! + index,
						};
					});

					return { items: mapped, totalCount: result.totalCount };
				},
			);
	}
}
