import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import SongApiContract from '@DataContracts/Song/SongApiContract';
import SongContract from '@DataContracts/Song/SongContract';
import { SongOptionalFields } from '@Models/EntryOptionalFields';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';
import SongType from '@Models/Songs/SongType';
import SongRepository from '@Repositories/SongRepository';
import GlobalValues from '@Shared/GlobalValues';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import AdvancedSearchFilters from '@Stores/Search/AdvancedSearchFilters';
import ArtistFilters from '@Stores/Search/ArtistFilters';
import _ from 'lodash';

import { IPlayListRepository, ISongForPlayList } from './PlayListStore';

export interface ISongSearchStore {
	searchTerm: string;
	sort: string;
	songType: string;
	afterDate?: Date;
	beforeDate?: Date;
	tagIds: number[];
	childTags: boolean;
	unifyEntryTypesAndTags: boolean;
	artistFilters: ArtistFilters;
	releaseEvent: BasicEntryLinkStore<IEntryWithIdAndName>;
	pvsOnly: boolean;
	since?: number;
	minScore?: number;
	onlyRatedSongs: boolean;
	parentVersion: BasicEntryLinkStore<SongContract>;
	fields: string;
	draftsOnly: boolean;
	advancedFilters: AdvancedSearchFilters;
}

export default class PlayListRepositoryForSongsAdapter
	implements IPlayListRepository {
	public constructor(
		private readonly values: GlobalValues,
		private readonly songRepo: SongRepository,
		private readonly songSearchStore: ISongSearchStore,
	) {}

	public getSongs = (
		pvServices: string,
		paging: PagingProperties,
		fields: SongOptionalFields,
		lang: ContentLanguagePreference,
	): Promise<PartialFindResultContract<ISongForPlayList>> =>
		this.songRepo
			.getList({
				paging: paging,
				lang: lang,
				query: this.songSearchStore.searchTerm,
				sort: this.songSearchStore.sort,
				songTypes:
					this.songSearchStore.songType !== SongType[SongType.Unspecified]
						? this.songSearchStore.songType
						: undefined,
				afterDate: this.songSearchStore.afterDate,
				beforeDate: this.songSearchStore.beforeDate,
				tagIds: this.songSearchStore.tagIds,
				childTags: this.songSearchStore.childTags,
				unifyTypesAndTags: this.songSearchStore.unifyEntryTypesAndTags,
				artistIds: this.songSearchStore.artistFilters.artistIds,
				artistParticipationStatus: this.songSearchStore.artistFilters
					.artistParticipationStatus,
				childVoicebanks: this.songSearchStore.artistFilters.childVoicebanks,
				includeMembers: this.songSearchStore.artistFilters.includeMembers,
				eventId: this.songSearchStore.releaseEvent.id,
				onlyWithPvs: this.songSearchStore.pvsOnly,
				pvServices: pvServices,
				since: this.songSearchStore.since,
				minScore: this.songSearchStore.minScore,
				userCollectionId: this.songSearchStore.onlyRatedSongs
					? this.values.loggedUserId
					: undefined,
				parentSongId: this.songSearchStore.parentVersion.id,
				fields: this.songSearchStore.fields,
				status: this.songSearchStore.draftsOnly ? 'Draft' : undefined,
				advancedFilters: this.songSearchStore.advancedFilters.filters,
				minMilliBpm: undefined,
				maxMilliBpm: undefined,
				minLength: undefined,
				maxLength: undefined,
			})
			.then((result: PartialFindResultContract<SongApiContract>) => {
				const mapped = _.map(result.items, (song, idx) => {
					return {
						name: song.name,
						song: song,
						indexInPlayList: paging.start! + idx,
					};
				});

				return { items: mapped, totalCount: result.totalCount };
			});
}
