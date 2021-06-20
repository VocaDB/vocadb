import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import { SongOptionalFields } from '@Models/EntryOptionalFields';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import SongType from '@Models/Songs/SongType';
import SongListRepository from '@Repositories/SongListRepository';
import { Computed, Observable, ObservableArray } from 'knockout';
import _ from 'lodash';

import AdvancedSearchFilter from '../../Search/AdvancedSearchFilter';
import { IPlayListRepository } from './PlayListViewModel';
import { ISongForPlayList } from './PlayListViewModel';

export default class PlayListRepositoryForSongListAdapter
	implements IPlayListRepository {
	public constructor(
		private songListRepo: SongListRepository,
		private songListId: number,
		private query: Observable<string>,
		private songType: Observable<string>,
		private tagIds: Computed<number[]>,
		private childTags: Observable<boolean>,
		private artistIds: Computed<number[]>,
		private artistParticipationStatus: Observable<string>,
		private childVoicebanks: Observable<boolean>,
		private advancedFilters: ObservableArray<AdvancedSearchFilter>,
		private sort: Observable<string>,
	) {}

	public getSongs = (
		pvServices: string,
		paging: PagingProperties,
		fields: SongOptionalFields,
		lang: ContentLanguagePreference,
	): Promise<PartialFindResultContract<ISongForPlayList>> =>
		this.songListRepo
			.getSongs({
				listId: this.songListId,
				query: this.query(),
				songTypes:
					this.songType() !== SongType[SongType.Unspecified]
						? this.songType()
						: undefined,
				tagIds: this.tagIds(),
				childTags: this.childTags(),
				artistIds: this.artistIds(),
				artistParticipationStatus: this.artistParticipationStatus(),
				childVoicebanks: this.childVoicebanks(),
				advancedFilters: this.advancedFilters(),
				pvServices: pvServices,
				paging: paging,
				fields: fields,
				sort: this.sort(),
				lang: lang,
			})
			.then((result) => {
				var mapped = _.map(result.items, (song, idx) => {
					return {
						name:
							song.order +
							'. ' +
							song.song.name +
							(song.notes ? ' (' + song.notes + ')' : ''),
						song: song.song,
						indexInPlayList: paging.start! + idx,
					};
				});

				return { items: mapped, totalCount: result.totalCount };
			});
}
