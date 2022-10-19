import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { SongInListContract } from '@/DataContracts/Song/SongInListContract';
import { SongListContract } from '@/DataContracts/Song/SongListContract';
import { SongListForEditContract } from '@/DataContracts/Song/SongListForEditContract';
import { SongListBaseContract } from '@/DataContracts/SongListBaseContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVService } from '@/Models/PVs/PVService';
import { SongType } from '@/Models/Songs/SongType';
import { EntryCommentRepository } from '@/Repositories/EntryCommentRepository';
import { SongOptionalField } from '@/Repositories/SongRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';

export interface SongListGetSongsQueryParams {
	listId: number;
	query: string;
	songTypes?: SongType[];
	tagIds: number[];
	childTags: boolean;
	artistIds: number[];
	artistParticipationStatus: string;
	childVoicebanks: boolean;
	advancedFilters: AdvancedSearchFilter[];
	sort: string;
}

export enum SongListOptionalField {
	'Description' = 'Description',
	'Events' = 'Events',
	'MainPicture' = 'MainPicture',
	'Tags' = 'Tags',
}

export class SongListRepository {
	public constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	public delete = ({
		id,
		notes,
		hardDelete,
	}: {
		id: number;
		notes: string;
		hardDelete: boolean;
	}): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(
				`/api/songLists/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
					notes,
				)}`,
			),
		);
	};

	// eslint-disable-next-line no-empty-pattern
	public getComments = ({}: {}): EntryCommentRepository =>
		new EntryCommentRepository(this.httpClient, this.urlMapper, '/songLists/');

	public getFeatured = ({
		query,
		category,
		paging,
		tagIds,
		fields,
		sort,
	}: {
		query: string;
		category: string;
		paging: PagingProperties;
		tagIds: number[];
		fields: SongListOptionalField[];
		sort: string;
	}): Promise<PartialFindResultContract<SongListContract>> => {
		var url = this.urlMapper.mapRelative('/api/songLists/featured');
		return this.httpClient.get<PartialFindResultContract<SongListContract>>(
			url,
			{
				query: query,
				featuredCategory: category,
				start: paging.start,
				getTotalCount: paging.getTotalCount,
				maxResults: paging.maxEntries,
				tagId: tagIds,
				fields: fields.join(','),
				sort: sort,
			},
		);
	};

	public getForEdit = ({
		id,
	}: {
		id: number;
	}): Promise<SongListForEditContract> => {
		var url = this.urlMapper.mapRelative(`/api/songLists/${id}/for-edit`);
		return this.httpClient.get<SongListForEditContract>(url);
	};

	public getSongs = ({
		fields,
		lang,
		paging,
		pvServices,
		queryParams,
	}: {
		fields: SongOptionalField[];
		lang: ContentLanguagePreference;
		paging: PagingProperties;
		pvServices?: PVService[];
		queryParams: SongListGetSongsQueryParams;
	}): Promise<PartialFindResultContract<SongInListContract>> => {
		const {
			listId,
			query,
			songTypes,
			tagIds,
			childTags,
			artistIds,
			artistParticipationStatus,
			childVoicebanks,
			advancedFilters,
			sort,
		} = queryParams;

		var url = this.urlMapper.mapRelative(`/api/songLists/${listId}/songs`);
		var data = {
			query: query,
			songTypes: songTypes?.join(','),
			tagId: tagIds,
			childTags: childTags,
			artistId: artistIds,
			artistParticipationStatus: artistParticipationStatus,
			childVoicebanks: childVoicebanks,
			advancedFilters: advancedFilters,
			pvServices: pvServices?.join(','),
			start: paging.start,
			getTotalCount: paging.getTotalCount,
			maxResults: paging.maxEntries,
			fields: fields.join(','),
			lang: lang,
			sort: sort,
		};

		return this.httpClient.get<PartialFindResultContract<SongInListContract>>(
			url,
			data,
		);
	};

	public getDetails = ({ id }: { id: number }): Promise<SongListContract> => {
		return this.httpClient.get<SongListContract>(
			this.urlMapper.mapRelative(`/api/songLists/${id}/details`),
		);
	};

	public getSongListWithArchivedVersions = ({
		id,
	}: {
		id: number;
	}): Promise<EntryWithArchivedVersionsContract<SongListContract>> => {
		return this.httpClient.get<
			EntryWithArchivedVersionsContract<SongListContract>
		>(this.urlMapper.mapRelative(`/api/songLists/${id}/versions`));
	};

	public getOne = ({ id }: { id: number }): Promise<SongListBaseContract> => {
		return this.httpClient.get<SongListBaseContract>(`/api/songLists/${id}`);
	};

	public edit = (
		requestToken: string,
		contract: SongListForEditContract,
		thumbPicUpload: File | undefined,
	): Promise<number> => {
		const formData = new FormData();
		formData.append('contract', JSON.stringify(contract));

		if (thumbPicUpload) formData.append('thumbPicUpload', thumbPicUpload);

		return this.httpClient.post<number>(
			this.urlMapper.mapRelative(`/api/songLists/${contract.id}`),
			formData,
			{
				headers: {
					'Content-Type': 'multipart/form-data',
					requestVerificationToken: requestToken,
				},
			},
		);
	};
}
