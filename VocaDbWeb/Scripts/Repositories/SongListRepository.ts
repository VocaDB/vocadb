
import AdvancedSearchFilter from '../ViewModels/Search/AdvancedSearchFilter';
import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import EntryCommentRepository from './EntryCommentRepository';
import PagingProperties from '../DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import SongInListContract from '../DataContracts/Song/SongInListContract';
import SongListContract from '../DataContracts/Song/SongListContract';
import SongListForEditContract from '../DataContracts/Song/SongListForEditContract';
import { SongOptionalFields } from '../Models/EntryOptionalFields';
import UrlMapper from '../Shared/UrlMapper';

//module vdb.repositories {

	export default class SongListRepository {

		constructor(private readonly urlMapper: UrlMapper) {}

		public delete = (id: number, notes: string, hardDelete: boolean, callback?: () => void) => {
			$.ajax(this.urlMapper.mapRelative("/api/songLists/" + id + "?hardDelete=" + hardDelete + "&notes=" + encodeURIComponent(notes)), { type: 'DELETE', success: callback });
		}

		public getComments = () => new EntryCommentRepository(this.urlMapper, "/songLists/");

		public getFeatured = (
			query: string,
			category: string,
			paging: PagingProperties,
			tagIds: number[],
			sort: string,
			callback: (result: PartialFindResultContract<SongListContract>) => void) => {
			
			var url = this.urlMapper.mapRelative("/api/songLists/featured");
			$.getJSON(url, {
				query: query,
				featuredCategory: category,
				start: paging.start, getTotalCount: paging.getTotalCount, maxResults: paging.maxEntries,
				tagId: tagIds,
				sort: sort
			}, callback);

		}

		public getForEdit = (id: number, callback: (result: SongListForEditContract) => void) => {

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
			advancedFilters: AdvancedSearchFilter[],
			pvServices: string,
			paging: PagingProperties,
			fields: SongOptionalFields,
			sort: string,
			lang: ContentLanguagePreference,
			callback: (result: PartialFindResultContract<SongInListContract>) => void) => {

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
				lang: ContentLanguagePreference[lang],
				sort: sort
			};

			$.getJSON(url, data, callback);

		};

	}

//}