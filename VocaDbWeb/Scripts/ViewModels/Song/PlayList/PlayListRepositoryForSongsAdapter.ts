import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import SongApiContract from '@DataContracts/Song/SongApiContract';
import { SongOptionalFields } from '@Models/EntryOptionalFields';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import SongType from '@Models/Songs/SongType';
import SongRepository from '@Repositories/SongRepository';
import { Computed, Observable, ObservableArray } from 'knockout';
import _ from 'lodash';

import AdvancedSearchFilter from '../../Search/AdvancedSearchFilter';
import { IPlayListRepository } from './PlayListViewModel';
import { ISongForPlayList } from './PlayListViewModel';

export default class PlayListRepositoryForSongsAdapter
	implements IPlayListRepository {
	constructor(
		private songRepo: SongRepository,
		private query: Observable<string>,
		private sort: Observable<string>,
		private songType: Observable<string>,
		private afterDate: Computed<Date>,
		private beforeDate: () => Date,
		private tagIds: Computed<number[]>,
		private childTags: Observable<boolean>,
		private unifyTypesAndTags: Observable<boolean>,
		private artistIds: Computed<number[]>,
		private artistParticipationStatus: Observable<string>,
		private childVoicebanks: Observable<boolean>,
		private includeMembers: Observable<boolean>,
		private eventId: Computed<number>,
		private onlyWithPvs: Observable<boolean>,
		private since: Observable<number>,
		private minScore: Observable<number>,
		private onlyRatedSongs: Observable<boolean>,
		private userCollectionId: number,
		private parentVersionId: Computed<number>,
		private fields: Computed<string>,
		private draftsOnly: Observable<boolean>,
		private advancedFilters: ObservableArray<AdvancedSearchFilter>,
	) {}

	public getSongs = (
		pvServices: string,
		paging: PagingProperties,
		fields: SongOptionalFields,
		lang: ContentLanguagePreference,
		callback: (result: PartialFindResultContract<ISongForPlayList>) => void,
	): void => {
		this.songRepo
			.getList(
				paging,
				lang,
				this.query(),
				this.sort(),
				this.songType() !== SongType[SongType.Unspecified]
					? this.songType()
					: null!,
				this.afterDate(),
				this.beforeDate(),
				this.tagIds(),
				this.childTags(),
				this.unifyTypesAndTags(),
				this.artistIds(),
				this.artistParticipationStatus(),
				this.childVoicebanks(),
				this.includeMembers(),
				this.eventId(),
				this.onlyWithPvs(),
				pvServices,
				this.since(),
				this.minScore(),
				this.onlyRatedSongs() ? this.userCollectionId : null!,
				this.parentVersionId(),
				this.fields(),
				this.draftsOnly() ? 'Draft' : null!,
				this.advancedFilters ? this.advancedFilters() : null!,
				null!,
				null!,
				null!,
				null!,
			)
			.then((result: PartialFindResultContract<SongApiContract>) => {
				var mapped = _.map(result.items, (song, idx) => {
					return {
						name: song.name,
						song: song,
						indexInPlayList: paging.start! + idx,
					};
				});

				callback({ items: mapped, totalCount: result.totalCount });
			});
	};
}
