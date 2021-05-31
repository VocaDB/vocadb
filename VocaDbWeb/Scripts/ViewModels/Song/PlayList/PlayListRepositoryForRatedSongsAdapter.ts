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
	constructor(
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
		callback: (result: PartialFindResultContract<ISongForPlayList>) => void,
	): void => {
		this.userRepo
			.getRatedSongsList(
				this.userId,
				paging,
				lang,
				this.query(),
				this.tagIds(),
				this.artistIds(),
				this.childVoicebanks(),
				this.rating(),
				this.songListId()!,
				this.advancedFilters(),
				this.groupByRating(),
				pvServices,
				'ThumbUrl',
				this.sort(),
			)
			.then(
				(result: PartialFindResultContract<RatedSongForUserForApiContract>) => {
					var mapped = _.map(result.items, (song, idx) => {
						return {
							name: song.song!.name,
							song: song.song!,
							indexInPlayList: paging.start! + idx,
						};
					});

					callback({
						items: mapped,
						totalCount: result.totalCount,
					});
				},
			);
	};
}
