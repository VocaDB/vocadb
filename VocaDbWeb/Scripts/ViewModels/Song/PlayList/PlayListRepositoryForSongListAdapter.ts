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
	constructor(
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
		callback: (result: PartialFindResultContract<ISongForPlayList>) => void,
	): void => {
		this.songListRepo
			.getSongs(
				this.songListId,
				this.query(),
				this.songType() !== SongType[SongType.Unspecified]
					? this.songType()
					: null!,
				this.tagIds(),
				this.childTags(),
				this.artistIds(),
				this.artistParticipationStatus(),
				this.childVoicebanks(),
				this.advancedFilters(),
				pvServices,
				paging,
				fields,
				this.sort(),
				lang,
			)
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

				callback({ items: mapped, totalCount: result.totalCount });
			});
	};
}
