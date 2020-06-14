
module vdb.repositories {

	import cls = vdb.models;
	import dc = vdb.dataContracts;

	export class SongListRepository {

		constructor(private readonly urlMapper: vdb.UrlMapper) {}

		public delete = (id: number, notes: string, hardDelete: boolean, callback?: () => void) => {
			$.ajax(this.urlMapper.mapRelative("/api/songLists/" + id + "?hardDelete=" + hardDelete + "&notes=" + encodeURIComponent(notes)), { type: 'DELETE', success: callback });
		}

		public getComments = () => new EntryCommentRepository(this.urlMapper, "/songLists/");

		public getFeatured = (
			query: string,
			category: string,
			paging: dc.PagingProperties,
			tagIds: number[],
			fields: string,
			sort: string,
			callback: (result: dc.PartialFindResultContract<dc.SongListContract>) => void) => {
			
			var url = this.urlMapper.mapRelative("/api/songLists/featured");
			$.getJSON(url, {
				query: query,
				featuredCategory: category,
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				tagId: tagIds,
				fields: fields,
				sort: sort
			}, callback);

		}

		public getForEdit = (id: number, callback: (result: dc.songs.SongListForEditContract) => void) => {

			var url = this.urlMapper.mapRelative("/api/songLists/" + id + "/for-edit");
			$.getJSON(url, callback);

		}

		public getSongs = (
			listId: number,
			query: string,
			songTypes: string,
			tagIds: number[],
			childTags: boolean,
			artistIds: number[],
			artistParticipationStatus: string,
			childVoicebanks: boolean,
			advancedFilters: viewModels.search.AdvancedSearchFilter[],
			pvServices: string,
			paging: dc.PagingProperties,
			fields: cls.SongOptionalFields,
			sort: string,
			lang: cls.globalization.ContentLanguagePreference,
			callback: (result: dc.PartialFindResultContract<dc.songs.SongInListContract>) => void) => {

			var url = this.urlMapper.mapRelative("/api/songLists/" + listId + "/songs");
			var data = {
				query: query,
				songTypes: songTypes,
				tagId: tagIds,
				childTags: childTags,
				artistId: artistIds,
				artistParticipationStatus: artistParticipationStatus,
				childVoicebanks: childVoicebanks,
				advancedFilters: advancedFilters,
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