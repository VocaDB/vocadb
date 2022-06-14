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

import { IPlayListRepository, ISongForPlayList } from './PlayListStore';

export interface ISongsAdapterStore {
	searchTerm: string;
	sort: string;
	songType: SongType;
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
		private readonly songsAdapterStore: ISongsAdapterStore,
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
				query: this.songsAdapterStore.searchTerm,
				sort: this.songsAdapterStore.sort,
				songTypes:
					this.songsAdapterStore.songType !== SongType.Unspecified
						? [this.songsAdapterStore.songType]
						: undefined,
				afterDate: this.songsAdapterStore.afterDate,
				beforeDate: this.songsAdapterStore.beforeDate,
				tagIds: this.songsAdapterStore.tagIds,
				childTags: this.songsAdapterStore.childTags,
				unifyTypesAndTags: this.songsAdapterStore.unifyEntryTypesAndTags,
				artistIds: this.songsAdapterStore.artistFilters.artistIds,
				artistParticipationStatus: this.songsAdapterStore.artistFilters
					.artistParticipationStatus,
				childVoicebanks: this.songsAdapterStore.artistFilters.childVoicebanks,
				includeMembers: this.songsAdapterStore.artistFilters.includeMembers,
				eventId: this.songsAdapterStore.releaseEvent.id,
				onlyWithPvs: this.songsAdapterStore.pvsOnly,
				pvServices: pvServices,
				since: this.songsAdapterStore.since,
				minScore: this.songsAdapterStore.minScore,
				userCollectionId: this.songsAdapterStore.onlyRatedSongs
					? this.values.loggedUserId
					: undefined,
				parentSongId: this.songsAdapterStore.parentVersion.id,
				fields: this.songsAdapterStore.fields,
				status: this.songsAdapterStore.draftsOnly ? 'Draft' : undefined,
				advancedFilters: this.songsAdapterStore.advancedFilters.filters,
				minMilliBpm: undefined,
				maxMilliBpm: undefined,
				minLength: undefined,
				maxLength: undefined,
			})
			.then((result: PartialFindResultContract<SongApiContract>) => {
				const mapped = result.items.map((song, idx) => ({
					name: song.name,
					song: song,
					indexInPlayList: paging.start! + idx,
				}));

				return { items: mapped, totalCount: result.totalCount };
			});
}
