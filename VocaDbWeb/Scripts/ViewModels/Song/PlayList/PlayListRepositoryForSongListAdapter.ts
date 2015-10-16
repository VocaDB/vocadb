
module vdb.viewModels.songs {
	
	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class PlayListRepositoryForSongListAdapter implements IPlayListRepository {

		constructor(private songListRepo: rep.SongListRepository, private songListId: number, private query: KnockoutObservable<string>, private sort: KnockoutObservable<string>) { }

		public getSongs = (
			pvServices: string,
			paging: dc.PagingProperties,
			fields: cls.SongOptionalFields,
			lang: cls.globalization.ContentLanguagePreference,
			callback: (result: dc.PartialFindResultContract<ISongForPlayList>) => void) => {

			this.songListRepo.getSongs(this.songListId, this.query(), pvServices, paging, fields, this.sort(), lang, result => {

				var mapped = _.map(result.items, (song, idx) => {
					return {
						name: song.order + ". " + song.song.name + (song.notes ? " (" + song.notes + ")" : ""),
						song: song.song,
						indexInPlayList: paging.start + idx
					}
				});

				callback({ items: mapped, totalCount: result.totalCount });

			});

		}

	}

}