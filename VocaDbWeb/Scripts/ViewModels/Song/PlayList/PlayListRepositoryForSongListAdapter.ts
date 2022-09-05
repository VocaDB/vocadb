import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVService } from '@/Models/PVs/PVService';
import { SongType } from '@/Models/Songs/SongType';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { SongOptionalField } from '@/Repositories/SongRepository';
import { AdvancedSearchFilter } from '@/ViewModels/Search/AdvancedSearchFilter';
import {
	IPlayListRepository,
	ISongForPlayList,
} from '@/ViewModels/Song/PlayList/PlayListViewModel';
import { Computed, Observable, ObservableArray } from 'knockout';

export class PlayListRepositoryForSongListAdapter
	implements IPlayListRepository {
	public constructor(
		private songListRepo: SongListRepository,
		private songListId: number,
		private query: Observable<string>,
		private songType: Observable<SongType>,
		private tagIds: Computed<number[]>,
		private childTags: Observable<boolean>,
		private artistIds: Computed<number[]>,
		private artistParticipationStatus: Observable<string>,
		private childVoicebanks: Observable<boolean>,
		private advancedFilters: ObservableArray<AdvancedSearchFilter>,
		private sort: Observable<string>,
	) {}

	public getSongs = (
		pvServices: PVService[],
		paging: PagingProperties,
		fields: SongOptionalField[],
		lang: ContentLanguagePreference,
	): Promise<PartialFindResultContract<ISongForPlayList>> =>
		this.songListRepo
			.getSongs({
				fields: fields,
				lang: lang,
				paging: paging,
				pvServices: pvServices,
				queryParams: {
					listId: this.songListId,
					query: this.query(),
					songTypes:
						this.songType() !== SongType.Unspecified
							? [this.songType()]
							: undefined,
					tagIds: this.tagIds(),
					childTags: this.childTags(),
					artistIds: this.artistIds(),
					artistParticipationStatus: this.artistParticipationStatus(),
					childVoicebanks: this.childVoicebanks(),
					advancedFilters: this.advancedFilters(),
					sort: this.sort(),
				},
			})
			.then((result) => {
				var mapped = result.items.map((song, idx) => ({
					name:
						song.order +
						'. ' +
						song.song.name +
						(song.notes ? ' (' + song.notes + ')' : ''),
					song: song.song,
					indexInPlayList: paging.start! + idx,
				}));

				return { items: mapped, totalCount: result.totalCount };
			});
}
