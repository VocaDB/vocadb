
module vdb.viewModels.songs {

	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class PlayListRepositoryForRatedSongsAdapter implements IPlayListRepository {

		constructor(private userRepo: rep.UserRepository,
			private userId: number,
			private query: KnockoutObservable<string>,
			private sort: KnockoutObservable<string>,
			private tag: KnockoutObservable<string>,
			private artistId: KnockoutObservable<number>,
			private childVoicebanks: KnockoutObservable<boolean>,
			private rating: KnockoutObservable<string>,
			private songListId: KnockoutObservable<number>,
			private groupByRating: KnockoutObservable<boolean>,
			private fields: KnockoutObservable<string>) { }

		public getSongs = (
			pvServices: string,
			paging: dc.PagingProperties,
			fields: cls.SongOptionalFields,
			lang: cls.globalization.ContentLanguagePreference,
			callback: (result: dc.PartialFindResultContract<ISongForPlayList>) => void) => {

			this.userRepo.getRatedSongsList(this.userId, paging, cls.globalization.ContentLanguagePreference[lang],
				this.query(),
				this.tag(),
				this.artistId(),
				this.childVoicebanks(),
				this.rating(),
				this.songListId(),
				this.groupByRating(),
				pvServices,
				"ThumbUrl",
				this.sort(),
				(result: dc.PartialFindResultContract<dc.RatedSongForUserForApiContract>) => {

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

} 