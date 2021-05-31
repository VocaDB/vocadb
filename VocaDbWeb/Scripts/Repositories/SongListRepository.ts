import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import SongInListContract from '@DataContracts/Song/SongInListContract';
import SongListContract from '@DataContracts/Song/SongListContract';
import SongListForEditContract from '@DataContracts/Song/SongListForEditContract';
import { SongOptionalFields } from '@Models/EntryOptionalFields';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import HttpClient from '@Shared/HttpClient';
import AdvancedSearchFilter from '@ViewModels/Search/AdvancedSearchFilter';

import EntryCommentRepository from './EntryCommentRepository';

export default class SongListRepository {
	constructor(private readonly httpClient: HttpClient) {}

	public delete = (
		id: number,
		notes: string,
		hardDelete: boolean,
	): Promise<void> => {
		return this.httpClient.delete<void>(
			`/api/songLists/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
				notes,
			)}`,
		);
	};

	public getComments = (): EntryCommentRepository =>
		new EntryCommentRepository(this.httpClient, '/songLists/');

	public getFeatured = (
		query: string,
		category: string,
		paging: PagingProperties,
		tagIds: number[],
		fields: string,
		sort: string,
	): Promise<PartialFindResultContract<SongListContract>> => {
		return this.httpClient.get<PartialFindResultContract<SongListContract>>(
			'/api/songLists/featured',
			{
				query: query,
				featuredCategory: category,
				start: paging.start,
				getTotalCount: paging.getTotalCount,
				maxResults: paging.maxEntries,
				tagId: tagIds,
				fields: fields,
				sort: sort,
			},
		);
	};

	public getForEdit = (id: number): Promise<SongListForEditContract> => {
		return this.httpClient.get<SongListForEditContract>(
			`/api/songLists/${id}/for-edit`,
		);
	};

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
	): Promise<PartialFindResultContract<SongInListContract>> => {
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
			start: paging.start,
			getTotalCount: paging.getTotalCount,
			maxResults: paging.maxEntries,
			fields: fields.fields,
			lang: ContentLanguagePreference[lang],
			sort: sort,
		};

		return this.httpClient.get<PartialFindResultContract<SongInListContract>>(
			`/api/songLists/${listId}/songs`,
			data,
		);
	};
}
