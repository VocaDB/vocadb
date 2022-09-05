import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { SongContract } from '@/DataContracts/Song/SongContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { IEntryWithIdAndName } from '@/Models/IEntryWithIdAndName';
import { PVService } from '@/Models/PVs/PVService';
import { SongType } from '@/Models/Songs/SongType';
import {
	SongOptionalField,
	SongRepository,
} from '@/Repositories/SongRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { AdvancedSearchFilters } from '@/Stores/Search/AdvancedSearchFilters';
import { ArtistFilters } from '@/Stores/Search/ArtistFilters';
import {
	IPlayListRepository,
	ISongForPlayList,
} from '@/Stores/Song/PlayList/PlayListStore';

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

export class PlayListRepositoryForSongsAdapter implements IPlayListRepository {
	public constructor(
		private readonly values: GlobalValues,
		private readonly songRepo: SongRepository,
		private readonly songsAdapterStore: ISongsAdapterStore,
	) {}

	public getSongs = (
		pvServices: PVService[],
		paging: PagingProperties,
		fields: SongOptionalField[],
		lang: ContentLanguagePreference,
	): Promise<PartialFindResultContract<ISongForPlayList>> =>
		this.songRepo
			.getList({
				fields: this.songsAdapterStore.fields,
				lang: lang,
				paging: paging,
				pvServices: pvServices,
				queryParams: {
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
					since: this.songsAdapterStore.since,
					minScore: this.songsAdapterStore.minScore,
					userCollectionId: this.songsAdapterStore.onlyRatedSongs
						? this.values.loggedUserId
						: undefined,
					parentSongId: this.songsAdapterStore.parentVersion.id,
					status: this.songsAdapterStore.draftsOnly ? 'Draft' : undefined,
					advancedFilters: this.songsAdapterStore.advancedFilters.filters,
					minMilliBpm: undefined,
					maxMilliBpm: undefined,
					minLength: undefined,
					maxLength: undefined,
				},
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
