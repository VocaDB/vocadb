
module vdb.viewModels.songList {
	
	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class IPlayListRepositorySongs implements IPlayListRepository {

		constructor(private songRepo: rep.SongRepository,
			private query: KnockoutObservable<string>,
			private sort: KnockoutObservable<string>,
			private songType: KnockoutObservable<string>,
			private tag: KnockoutObservable<string>,
			private artistId: KnockoutObservable<number>,
			private artistParticipationStatus: KnockoutObservable<string>,
			private childVoicebanks: KnockoutObservable<boolean>,
			private onlyWithPvs: KnockoutObservable<boolean>,
			private since: KnockoutObservable<number>,
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

			this.songRepo.getList(paging, this.songRepo.languagePreferenceStr, this.query(), this.sort(),
				this.songType() != cls.songs.SongType[cls.songs.SongType.Unspecified] ? this.songType() : null,
				this.tag(),
				this.artistId(),
				this.artistParticipationStatus(),
				this.childVoicebanks(),
				this.onlyWithPvs(),
				this.since(),
				this.onlyRatedSongs() ? this.userCollectionId : null,
				this.fields(),
				this.draftsOnly() ? "Draft" : null,
				(result: dc.PartialFindResultContract<dc.SongApiContract>) => {

				var mapped = _.map(result.items, song => {
					return { name: song.name, song: song }
				});

				callback({ items: mapped, totalCount: result.totalCount });

			});

		}

	}

}