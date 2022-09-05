import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVService } from '@/Models/PVs/PVService';
import { SongType } from '@/Models/Songs/SongType';
import {
	SongOptionalField,
	SongRepository,
} from '@/Repositories/SongRepository';
import { AdvancedSearchFilter } from '@/ViewModels/Search/AdvancedSearchFilter';
import {
	IPlayListRepository,
	ISongForPlayList,
} from '@/ViewModels/Song/PlayList/PlayListViewModel';
import { Computed, Observable, ObservableArray } from 'knockout';

export class PlayListRepositoryForSongsAdapter implements IPlayListRepository {
	public constructor(
		private songRepo: SongRepository,
		private query: Observable<string>,
		private sort: Observable<string>,
		private songType: Observable<SongType>,
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
		pvServices: PVService[],
		paging: PagingProperties,
		fields: SongOptionalField[],
		lang: ContentLanguagePreference,
	): Promise<PartialFindResultContract<ISongForPlayList>> =>
		this.songRepo
			.getList({
				fields: this.fields(),
				lang: lang,
				paging: paging,
				pvServices: pvServices,
				queryParams: {
					query: this.query(),
					sort: this.sort(),
					songTypes:
						this.songType() !== SongType.Unspecified
							? [this.songType()]
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
					since: this.since(),
					minScore: this.minScore(),
					userCollectionId: this.onlyRatedSongs()
						? this.userCollectionId
						: undefined,
					parentSongId: this.parentVersionId(),
					status: this.draftsOnly() ? 'Draft' : undefined,
					advancedFilters: this.advancedFilters
						? this.advancedFilters()
						: undefined,
					minMilliBpm: undefined,
					maxMilliBpm: undefined,
					minLength: undefined,
					maxLength: undefined,
				},
			})
			.then((result: PartialFindResultContract<SongApiContract>) => {
				var mapped = result.items.map((song, idx) => ({
					name: song.name,
					song: song,
					indexInPlayList: paging.start! + idx,
				}));

				return { items: mapped, totalCount: result.totalCount };
			});
}
