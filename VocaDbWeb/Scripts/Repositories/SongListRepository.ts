
module vdb.repositories {

	import dc = vdb.dataContracts;

	export class SongListRepository {

		constructor(private urlMapper: vdb.UrlMapper) {}

		public getForEdit = (id: number, callback: (result: dc.songs.SongListForEditContract) => void) => {

			var url = this.urlMapper.mapRelative("/api/songLists/" + id + "/for-edit");
			$.getJSON(url, callback);

		}

		public getSongs = (
			listId: number,
			pvServices: string,
			paging: dc.PagingProperties, lang: string, callback: any) => {

			var url = this.urlMapper.mapRelative("/api/songLists/" + listId + "/songs");
			var data = {
				pvServices: pvServices,
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				fields: "ThumbUrl", lang: lang
			};

			$.getJSON(url, data, callback);

		};

	}

}