
module vdb.viewModels.songs {
	
	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class PlayListRepositoryForSongsAdapter implements IPlayListRepository {

		constructor(private songRepo: rep.SongRepository,
			private query: KnockoutObservable<string>,
			private sort: KnockoutObservable<string>,
			private songType: KnockoutObservable<string>,
			private tagIds: KnockoutObservable<number[]>,
			private childTags: KnockoutObservable<boolean>,
			private artistIds: KnockoutComputed<number[]>,
			private artistParticipationStatus: KnockoutObservable<string>,
			private childVoicebanks: KnockoutObservable<boolean>,
			private onlyWithPvs: KnockoutObservable<boolean>,
			private since: KnockoutObservable<number>,
			private minScore: KnockoutObservable<number>,
			private onlyRatedSongs: KnockoutObservable<boolean>,
			private userCollectionId: number,
			private fields: KnockoutObservable<string>,
			private draftsOnly: KnockoutObservable<boolean>) { }

		public getSongs = (
			pvServices: string,
			paging: dc.PagingProperties,
			fields: cls.SongOptionalFields,
			lang: cls.globalization.ContentLanguagePreference,
			callback: (result: dc.PartialFindResultContract<ISongForPlayList>) => void) => {

			this.songRepo.getList(paging, cls.globalization.ContentLanguagePreference[lang], this.query(), this.sort(),
				this.songType() != cls.songs.SongType[cls.songs.SongType.Unspecified] ? this.songType() : null,
				this.tagIds(),
				this.childTags(),
				this.artistIds(),
				this.artistParticipationStatus(),
				this.childVoicebanks(),
				this.onlyWithPvs(),
				pvServices,
				this.since(),
				this.minScore(),
				this.onlyRatedSongs() ? this.userCollectionId : null,
				this.fields(),
				this.draftsOnly() ? "Draft" : null,
				(result: dc.PartialFindResultContract<dc.SongApiContract>) => {

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

}