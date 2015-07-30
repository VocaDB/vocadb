
module vdb.repositories {

	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class SongListRepository {

		constructor(private urlMapper: vdb.UrlMapper) {}

		public getFeatured = (query: string, category: string, paging: dc.PagingProperties, sort: string,
			callback: (result: dc.PartialFindResultContract<dc.SongListContract>) => void) => {
			
			var url = this.urlMapper.mapRelative("/api/songLists/featured");
			$.getJSON(url, {
				query: query,
				featuredCategory: category,
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				sort: sort
			}, callback);

		}

		public getForEdit = (id: number, callback: (result: dc.songs.SongListForEditContract) => void) => {

			var url = this.urlMapper.mapRelative("/api/songLists/" + id + "/for-edit");
			$.getJSON(url, callback);

		}

		public getSongs = (
			listId: number,
			pvServices: string,
			paging: dc.PagingProperties,
			fields: cls.SongOptionalFields,
			sort: string,
			lang: cls.globalization.ContentLanguagePreference,
			callback: (result: dc.PartialFindResultContract<dc.songs.SongInListContract>) => void) => {

			var url = this.urlMapper.mapRelative("/api/songLists/" + listId + "/songs");
			var data = {
				pvServices: pvServices,
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				fields: fields.fields, 
				lang: cls.globalization.ContentLanguagePreference[lang],
				sort: sort
			};

			$.getJSON(url, data, callback);

		};

	}

}