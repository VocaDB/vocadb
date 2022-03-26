import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import { SongOptionalFields } from '@Models/EntryOptionalFields';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import SongType from '@Models/Songs/SongType';
import SongListRepository from '@Repositories/SongListRepository';
import AdvancedSearchFilters from '@Stores/Search/AdvancedSearchFilters';
import ArtistFilters from '@Stores/Search/ArtistFilters';
import _ from 'lodash';

import { IPlayListRepository } from './PlayListStore';
import { ISongForPlayList } from './PlayListStore';

export interface ISongListStore {
	query: string;
	songType: string /* TODO: enum */;
	tagIds: number[];
	childTags: boolean;
	artistFilters: ArtistFilters;
	advancedFilters: AdvancedSearchFilters;
	sort: string /* TODO: enum */;
}

export default class PlayListRepositoryForSongListAdapter
	implements IPlayListRepository {
	public constructor(
		private readonly songListRepo: SongListRepository,
		private readonly songListId: number,
		private readonly songListStore: ISongListStore,
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
				query: this.songListStore.query,
				songTypes:
					this.songListStore.songType !== SongType[SongType.Unspecified]
						? this.songListStore.songType
						: undefined,
				tagIds: this.songListStore.tagIds,
				childTags: this.songListStore.childTags,
				artistIds: this.songListStore.artistFilters.artistIds,
				artistParticipationStatus: this.songListStore.artistFilters
					.artistParticipationStatus,
				childVoicebanks: this.songListStore.artistFilters.childVoicebanks,
				advancedFilters: this.songListStore.advancedFilters.filters,
				pvServices: pvServices,
				paging: paging,
				fields: fields,
				sort: this.songListStore.sort,
				lang: lang,
			})
			.then((result) => {
				const mapped = _.map(result.items, (song, idx) => {
					return {
						name: `${song.order}. ${song.song.name}${
							song.notes ? ` (${song.notes})` : ''
						}`,
						song: song.song,
						indexInPlayList: paging.start! + idx,
					};
				});

				return { items: mapped, totalCount: result.totalCount };
			});
}
