import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ArchivedTagVersionDetailsContract } from '@/DataContracts/Tag/ArchivedTagVersionDetailsContract';
import { EntryTagMappingContract } from '@/DataContracts/Tag/EntryTagMappingContract';
import { TagApiContract } from '@/DataContracts/Tag/TagApiContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagCategoryContract } from '@/DataContracts/Tag/TagCategoryContract';
import { TagDetailsContract } from '@/DataContracts/Tag/TagDetailsContract';
import { TagForEditContract } from '@/DataContracts/Tag/TagForEditContract';
import { TagMappingContract } from '@/DataContracts/Tag/TagMappingContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { AjaxHelper } from '@/Helpers/AjaxHelper';
import { EntryType } from '@/Models/EntryType';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { NameMatchMode } from '@/Models/NameMatchMode';
import {
	BaseRepository,
	CommonQueryParams,
} from '@/Repositories/BaseRepository';
import { EntryCommentRepository } from '@/Repositories/EntryCommentRepository';
import { functions } from '@/Shared/GlobalFunctions';
import { httpClient, HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { vdbConfig } from '@/vdbConfig';
import qs from 'qs';

export enum TagOptionalField {
	'AliasedTo' = 'AliasedTo',
	'AdditionalNames' = 'AdditionalNames',
	'Description' = 'Description',
	'MainPicture' = 'MainPicture',
	'Names' = 'Names',
	'Parent' = 'Parent',
	'RelatedTags' = 'RelatedTags',
	'TranslatedDescription' = 'TranslatedDescription',
	'WebLinks' = 'WebLinks',
}

export class TagRepository extends BaseRepository {
	private readonly urlMapper: UrlMapper;

	constructor(private readonly httpClient: HttpClient, baseUrl: string) {
		super(baseUrl);
		this.urlMapper = new UrlMapper(baseUrl);
	}

	create = ({ name }: { name: string }): Promise<TagBaseContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/tags?name=${name}`);
		return this.httpClient.post<TagBaseContract>(url);
	};

	createReport = ({
		entryId: tagId,
		reportType,
		notes,
		versionNumber,
	}: {
		entryId: number;
		reportType: string;
		notes: string;
		versionNumber?: number;
	}): Promise<void> => {
		var url = functions.mergeUrls(
			this.baseUrl,
			`/api/tags/${tagId}/reports?${AjaxHelper.createUrl({
				reportType: [reportType],
				notes: [notes],
				versionNumber: [versionNumber!],
			})}`,
		);
		return this.httpClient.post<void>(url);
	};

	getById = ({
		id,
		fields,
		lang,
	}: {
		id: number;
		fields?: TagOptionalField[];
		lang?: ContentLanguagePreference;
	}): Promise<TagApiContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/tags/${id}`);
		return this.httpClient.get<TagApiContract>(url, {
			fields: fields?.join(','),
			lang: lang,
		});
	};

	getByName = ({
		name,
		lang,
	}: {
		name: string;
		lang?: ContentLanguagePreference;
	}): Promise<TagApiContract> => {
		return this.httpClient.get<TagApiContract>(
			this.urlMapper.mapRelative(`/api/tags/byName/${name}`),
			{ lang: lang },
		);
	};

	// eslint-disable-next-line no-empty-pattern
	getComments = ({}: {}): EntryCommentRepository =>
		new EntryCommentRepository(
			this.httpClient,
			new UrlMapper(this.baseUrl),
			'/tags/',
		);

	getEntryTypeTag = ({
		entryType,
		subType,
		lang,
	}: {
		entryType: EntryType;
		subType: string;
		lang: ContentLanguagePreference;
	}): Promise<TagApiContract> => {
		var url = functions.mergeUrls(
			this.baseUrl,
			`/api/entry-types/${entryType}/${subType}/tag`,
		);
		return this.httpClient.get<TagApiContract>(url, {
			fields: [TagOptionalField.Description].join(','),
			lang: lang,
		});
	};

	getList = ({
		queryParams,
	}: {
		queryParams: TagQueryParams;
	}): Promise<PartialFindResultContract<TagApiContract>> => {
		var nameMatchMode = queryParams.nameMatchMode || NameMatchMode.Auto;

		var url = functions.mergeUrls(this.baseUrl, '/api/tags');
		var data = {
			start: queryParams.start,
			getTotalCount: queryParams.getTotalCount,
			maxResults: queryParams.maxResults,
			query: queryParams.query,
			fields: queryParams.fields?.join(','),
			nameMatchMode: NameMatchMode[nameMatchMode],
			allowAliases: queryParams.allowAliases,
			categoryName: queryParams.categoryName,
			lang: queryParams.lang,
			sort: queryParams.sort,
		};

		return this.httpClient.get<PartialFindResultContract<TagApiContract>>(
			url,
			data,
		);
	};

	// eslint-disable-next-line no-empty-pattern
	getEntryTagMappings = ({}: {}): Promise<EntryTagMappingContract[]> => {
		return this.httpClient.get<EntryTagMappingContract[]>(
			this.urlMapper.mapRelative('/api/tags/entry-type-mappings'),
		);
	};

	getMappings = ({
		paging,
	}: {
		paging: PagingProperties;
	}): Promise<PartialFindResultContract<TagMappingContract>> => {
		return this.httpClient.get<PartialFindResultContract<TagMappingContract>>(
			this.urlMapper.mapRelative('/api/tags/mappings'),
			paging,
		);
	};

	getTopTags = ({
		lang,
		categoryName,
		entryType,
	}: {
		lang: ContentLanguagePreference;
		categoryName?: string;
		entryType?: EntryType;
	}): Promise<TagBaseContract[]> => {
		var url = functions.mergeUrls(this.baseUrl, '/api/tags/top');
		var data = {
			lang: lang,
			categoryName: categoryName,
			entryType: entryType || undefined,
		};

		return this.httpClient.get<TagBaseContract[]>(url, data);
	};

	saveEntryMappings = ({
		mappings,
	}: {
		mappings: EntryTagMappingContract[];
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative('/api/tags/entry-type-mappings');
		return this.httpClient.put<void>(url, mappings);
	};

	saveMappings = ({
		mappings,
	}: {
		mappings: TagMappingContract[];
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative('/api/tags/mappings');
		return this.httpClient.put<void>(url, mappings);
	};

	getDetails = ({ id }: { id: number }): Promise<TagDetailsContract> => {
		return this.httpClient.get<TagDetailsContract>(
			this.urlMapper.mapRelative(`/api/tags/${id}/details`),
		);
	};

	getTagsByCategories = (): Promise<TagCategoryContract[]> => {
		return this.httpClient.get<TagCategoryContract[]>(
			this.urlMapper.mapRelative('/api/tags/by-categories'),
		);
	};

	getTagWithArchivedVersions = ({
		id,
	}: {
		id: number;
	}): Promise<EntryWithArchivedVersionsContract<TagApiContract>> => {
		return this.httpClient.get<
			EntryWithArchivedVersionsContract<TagApiContract>
		>(this.urlMapper.mapRelative(`/api/tags/${id}/versions`));
	};

	getVersionDetails = ({
		id,
		comparedVersionId,
	}: {
		id: number;
		comparedVersionId?: number;
	}): Promise<ArchivedTagVersionDetailsContract> => {
		return this.httpClient.get<ArchivedTagVersionDetailsContract>(
			this.urlMapper.mapRelative(`/api/tags/versions/${id}`),
			{ comparedVersionId: comparedVersionId },
		);
	};

	getForEdit = ({ id }: { id: number }): Promise<TagForEditContract> => {
		return this.httpClient.get<TagForEditContract>(
			this.urlMapper.mapRelative(`/api/tags/${id}/for-edit`),
		);
	};

	edit = (
		requestToken: string,
		contract: TagForEditContract,
		thumbPicUpload: File | undefined,
	): Promise<number> => {
		const formData = new FormData();
		formData.append('contract', JSON.stringify(contract));

		if (thumbPicUpload) formData.append('thumbPicUpload', thumbPicUpload);

		return this.httpClient.post<number>(
			this.urlMapper.mapRelative(`/api/tags/${contract.id}`),
			formData,
			{
				headers: {
					'Content-Type': 'multipart/form-data',
					requestVerificationToken: requestToken,
				},
			},
		);
	};

	merge = (
		requestToken: string,
		{ id, targetTagId }: { id: number; targetTagId: number },
	): Promise<void> => {
		return this.httpClient.post(
			this.urlMapper.mapRelative(
				`/api/tags/${id}/merge?${qs.stringify({ targetTagId: targetTagId })}`,
			),
			undefined,
			{
				headers: {
					requestVerificationToken: requestToken,
				},
			},
		);
	};

	delete = (
		requestToken: string,
		{
			id,
			notes,
			hardDelete,
		}: { id: number; notes: string; hardDelete: boolean },
	): Promise<void> => {
		return this.httpClient.delete(
			this.urlMapper.mapRelative(
				`/api/tags/${id}?${qs.stringify({
					id: id,
					notes: notes,
					hardDelete: hardDelete,
				})}`,
			),
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	updateVersionVisibility = (
		requestToken: string,
		{
			archivedVersionId,
			hidden,
		}: {
			archivedVersionId: number;
			hidden: boolean;
		},
	): Promise<void> => {
		return this.httpClient.post(
			this.urlMapper.mapRelative(
				`/api/tags/versions/${archivedVersionId}/update-visibility?${qs.stringify(
					{
						hidden: hidden,
					},
				)}`,
			),
			undefined,
			{ headers: { requestVerificationToken: requestToken } },
		);
	};
}

export interface TagQueryParams extends CommonQueryParams {
	allowAliases?: boolean;
	categoryName?: string;
	// Comma-separated list of optional fields
	fields?: TagOptionalField[];
	sort?: string;
}

export const tagRepo = new TagRepository(httpClient, vdbConfig.baseAddress);
