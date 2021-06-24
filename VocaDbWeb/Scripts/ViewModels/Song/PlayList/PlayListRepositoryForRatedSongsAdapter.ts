import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import RatedSongForUserForApiContract from '@DataContracts/User/RatedSongForUserForApiContract';
import { SongOptionalFields } from '@Models/EntryOptionalFields';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import UserRepository from '@Repositories/UserRepository';
import { Computed, Observable, ObservableArray } from 'knockout';
import _ from 'lodash';

import AdvancedSearchFilter from '../../Search/AdvancedSearchFilter';
import { IPlayListRepository } from './PlayListViewModel';
import { ISongForPlayList } from './PlayListViewModel';

export default class PlayListRepositoryForRatedSongsAdapter
	implements IPlayListRepository {
	public constructor(
		private userRepo: UserRepository,
		private userId: number,
		private query: Observable<string>,
		private sort: Observable<string>,
		private tagIds: Computed<number[]>,
		private artistIds: Computed<number[]>,
		private childVoicebanks: Observable<boolean>,
		private rating: Observable<string>,
		private songListId: Observable<number | null>,
		private advancedFilters: ObservableArray<AdvancedSearchFilter>,
		private groupByRating: Observable<boolean>,
		private fields: Observable<string>,
	) {}

	public getSongs = (
		pvServices: string,
		paging: PagingProperties,
		fields: SongOptionalFields,
		lang: ContentLanguagePreference,
	): Promise<PartialFindResultContract<ISongForPlayList>> =>
		this.userRepo
			.getRatedSongsList({
				userId: this.userId,
				paging: paging,
				lang: lang,
				query: this.query(),
				tagIds: this.tagIds(),
				artistIds: this.artistIds(),
				childVoicebanks: this.childVoicebanks(),
				rating: this.rating(),
				songListId: this.songListId()!,
				advancedFilters: this.advancedFilters(),
				groupByRating: this.groupByRating(),
				pvServices: pvServices,
				fields: 'ThumbUrl',
				sort: this.sort(),
			})
			.then(
				(result: PartialFindResultContract<RatedSongForUserForApiContract>) => {
					var mapped = _.map(result.items, (song, idx) => {
						return {
							name: song.song!.name,
							song: song.song!,
							indexInPlayList: paging.start! + idx,
						};
					});

					return {
						items: mapped,
						totalCount: result.totalCount,
					};
				},
			);
}
