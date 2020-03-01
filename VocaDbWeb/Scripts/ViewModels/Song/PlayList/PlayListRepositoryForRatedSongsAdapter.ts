import AdvancedSearchFilter from '../../Search/AdvancedSearchFilter';
import ContentLanguagePreference from '../../../Models/Globalization/ContentLanguagePreference';
import { IPlayListRepository } from './PlayListViewModel';
import { ISongForPlayList } from './PlayListViewModel';
import PagingProperties from '../../../DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '../../../DataContracts/PartialFindResultContract';
import RatedSongForUserForApiContract from '../../../DataContracts/User/RatedSongForUserForApiContract';
import { SongOptionalFields } from '../../../Models/EntryOptionalFields';
import UserRepository from '../../../Repositories/UserRepository';

//module vdb.viewModels.songs {

	export default class PlayListRepositoryForRatedSongsAdapter implements IPlayListRepository {

		constructor(private userRepo: UserRepository,
			private userId: number,
			private query: KnockoutObservable<string>,
			private sort: KnockoutObservable<string>,
			private tagIds: KnockoutObservable<number[]>,
			private artistIds: KnockoutComputed<number[]>,
			private childVoicebanks: KnockoutObservable<boolean>,
			private rating: KnockoutObservable<string>,
			private songListId: KnockoutObservable<number>,
			private advancedFilters: KnockoutObservableArray<AdvancedSearchFilter>,
			private groupByRating: KnockoutObservable<boolean>,
			private fields: KnockoutObservable<string>) { }

		public getSongs = (
			pvServices: string,
			paging: PagingProperties,
			fields: SongOptionalFields,
			lang: ContentLanguagePreference,
			callback: (result: PartialFindResultContract<ISongForPlayList>) => void) => {

			this.userRepo.getRatedSongsList(this.userId, paging, ContentLanguagePreference[lang],
				this.query(),
				this.tagIds(),
				this.artistIds(),
				this.childVoicebanks(),
				this.rating(),
				this.songListId(),
				this.advancedFilters(),
				this.groupByRating(),
				pvServices,
				"ThumbUrl",
				this.sort(),
				(result: PartialFindResultContract<RatedSongForUserForApiContract>) => {

					var mapped = _.map(result.items, (song, idx) => {
						return {
							name: song.song.name,
							song: song.song,
							indexInPlayList: paging.start + idx
						}
					});

					callback({ items: mapped, totalCount: result.totalCount });

				});

		}

	}

//} 