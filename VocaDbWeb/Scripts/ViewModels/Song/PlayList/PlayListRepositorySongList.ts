
module vdb.viewModels.songList {
	
	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class IPlayListRepositorySongList implements IPlayListRepository {

		constructor(private songListRepo: rep.SongListRepository, private songListId: number) { }

		public getSongs = (
			pvServices: string,
			paging: dc.PagingProperties,
			fields: cls.SongOptionalFields,
			lang: cls.globalization.ContentLanguagePreference,
			callback: (result: dc.PartialFindResultContract<ISongForPlayList>) => void) => {

			this.songListRepo.getSongs(this.songListId, pvServices, paging, fields, lang, result => {
				var mapped = _.map(result.items, song => {
					return {
						name: song.order + ". " + song.song.name + " (" + song.notes + ")",
						song: song.song
					}
				});
				callback({ items: mapped, totalCount: result.totalCount });
			});

		}

	}

}