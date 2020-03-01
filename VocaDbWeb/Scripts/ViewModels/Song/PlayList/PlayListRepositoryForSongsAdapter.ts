import AdvancedSearchFilter from '../../Search/AdvancedSearchFilter';
import ContentLanguagePreference from '../../../Models/Globalization/ContentLanguagePreference';
import { IPlayListRepository } from './PlayListViewModel';
import { ISongForPlayList } from './PlayListViewModel';
import { SongOptionalFields } from '../../../Models/EntryOptionalFields';
import PagingProperties from '../../../DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '../../../DataContracts/PartialFindResultContract';
import SongApiContract from '../../../DataContracts/Song/SongApiContract';
import SongRepository from '../../../Repositories/SongRepository';
import SongType from '../../../Models/Songs/SongType';

//module vdb.viewModels.songs {
	
	export default class PlayListRepositoryForSongsAdapter implements IPlayListRepository {

		constructor(private songRepo: SongRepository,
			private query: KnockoutObservable<string>,
			private sort: KnockoutObservable<string>,
			private songType: KnockoutObservable<string>,
			private afterDate: KnockoutObservable<Date>,
			private beforeDate: () => Date,
			private tagIds: KnockoutObservable<number[]>,
			private childTags: KnockoutObservable<boolean>,
			private unifyTypesAndTags: KnockoutObservable<boolean>,
			private artistIds: KnockoutComputed<number[]>,
			private artistParticipationStatus: KnockoutObservable<string>,
			private childVoicebanks: KnockoutObservable<boolean>,
			private includeMembers: KnockoutObservable<boolean>,
			private eventId: KnockoutComputed<number>,
			private onlyWithPvs: KnockoutObservable<boolean>,
			private since: KnockoutObservable<number>,
			private minScore: KnockoutObservable<number>,
			private onlyRatedSongs: KnockoutObservable<boolean>,
			private userCollectionId: number,
			private parentVersionId: KnockoutComputed<number>,
			private fields: KnockoutObservable<string>,
			private draftsOnly: KnockoutObservable<boolean>,
			private advancedFilters: KnockoutObservableArray<AdvancedSearchFilter>) { }

		public getSongs = (
			pvServices: string,
			paging: PagingProperties,
			fields: SongOptionalFields,
			lang: ContentLanguagePreference,
			callback: (result: PartialFindResultContract<ISongForPlayList>) => void) => {

			this.songRepo.getList(paging, ContentLanguagePreference[lang], this.query(), this.sort(),
				this.songType() != SongType[SongType.Unspecified] ? this.songType() : null,
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
				this.onlyRatedSongs() ? this.userCollectionId : null,
				this.parentVersionId(),
				this.fields(),
				this.draftsOnly() ? "Draft" : null,
				this.advancedFilters ? this.advancedFilters() : null,
				(result: PartialFindResultContract<SongApiContract>) => {

				var mapped = _.map(result.items, (song, idx) => {
					return {
						name: song.name,
						song: song,
						indexInPlayList: paging.start + idx
					}
				});

				callback({ items: mapped, totalCount: result.totalCount });

			});

		}

	}

//}