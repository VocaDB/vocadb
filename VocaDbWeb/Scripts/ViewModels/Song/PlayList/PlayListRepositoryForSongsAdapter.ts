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
	public constructor(
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
			.getList({
				paging: paging,
				lang: lang,
				query: this.query(),
				sort: this.sort(),
				songTypes:
					this.songType() !== SongType[SongType.Unspecified]
						? this.songType()
						: undefined,
				afterDate: this.afterDate(),
				beforeDate: this.beforeDate(),
				tagIds: this.tagIds(),
				childTags: this.childTags(),
				unifyTypesAndTags: this.unifyTypesAndTags(),
				artistIds: this.artistIds(),
				artistParticipationStatus: this.artistParticipationStatus(),
				childVoicebanks: this.childVoicebanks(),
				includeMembers: this.includeMembers(),
				eventId: this.eventId(),
				onlyWithPvs: this.onlyWithPvs(),
				pvServices: pvServices,
				since: this.since(),
				minScore: this.minScore(),
				userCollectionId: this.onlyRatedSongs()
					? this.userCollectionId
					: undefined,
				parentSongId: this.parentVersionId(),
				fields: this.fields(),
				status: this.draftsOnly() ? 'Draft' : undefined,
				advancedFilters: this.advancedFilters
					? this.advancedFilters()
					: undefined,
				minMilliBpm: undefined,
				maxMilliBpm: undefined,
				minLength: undefined,
				maxLength: undefined,
			})
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
