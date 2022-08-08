import PagingProperties from '@/DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@/DataContracts/PartialFindResultContract';
import EntryTagMappingContract from '@/DataContracts/Tag/EntryTagMappingContract';
import TagApiContract from '@/DataContracts/Tag/TagApiContract';
import TagBaseContract from '@/DataContracts/Tag/TagBaseContract';
import TagCategoryContract from '@/DataContracts/Tag/TagCategoryContract';
import TagDetailsContract from '@/DataContracts/Tag/TagDetailsContract';
import TagForEditContract from '@/DataContracts/Tag/TagForEditContract';
import TagMappingContract from '@/DataContracts/Tag/TagMappingContract';
import EntryWithArchivedVersionsContract from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import AjaxHelper from '@/Helpers/AjaxHelper';
import EntryType from '@/Models/EntryType';
import ContentLanguagePreference from '@/Models/Globalization/ContentLanguagePreference';
import NameMatchMode from '@/Models/NameMatchMode';
import BaseRepository, {
	CommonQueryParams,
} from '@/Repositories/BaseRepository';
import EntryCommentRepository from '@/Repositories/EntryCommentRepository';
import functions from '@/Shared/GlobalFunctions';
import HttpClient from '@/Shared/HttpClient';
import UrlMapper from '@/Shared/UrlMapper';

export default class TagRepository extends BaseRepository {
	private readonly urlMapper: UrlMapper;

	public constructor(private readonly httpClient: HttpClient, baseUrl: string) {
		super(baseUrl);
		this.urlMapper = new UrlMapper(baseUrl);
	}

	public create = ({ name }: { name: string }): Promise<TagBaseContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/tags?name=${name}`);
		return this.httpClient.post<TagBaseContract>(url);
	};

	public createReport = ({
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

	public getById = ({
		id,
		fields,
		lang,
	}: {
		id: number;
		fields?: string;
		lang?: ContentLanguagePreference;
	}): Promise<TagApiContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/tags/${id}`);
		return this.httpClient.get<TagApiContract>(url, {
			fields: fields || undefined,
			lang: lang,
		});
	};

	// eslint-disable-next-line no-empty-pattern
	public getComments = ({}: {}): EntryCommentRepository =>
		new EntryCommentRepository(
			this.httpClient,
			new UrlMapper(this.baseUrl),
			'/tags/',
		);

	public getEntryTypeTag = ({
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
			`/api/entry-types/${EntryType[entryType]}/${subType}/tag`,
		);
		return this.httpClient.get<TagApiContract>(url, {
			fields: 'Description',
			lang: lang,
		});
	};

	public getList = ({
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
			fields: queryParams.fields || undefined,
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
	public getEntryTagMappings = ({}: {}): Promise<EntryTagMappingContract[]> => {
		return this.httpClient.get<EntryTagMappingContract[]>(
			this.urlMapper.mapRelative('/api/tags/entry-type-mappings'),
		);
	};

	public getMappings = ({
		paging,
	}: {
		paging: PagingProperties;
	}): Promise<PartialFindResultContract<TagMappingContract>> => {
		return this.httpClient.get<PartialFindResultContract<TagMappingContract>>(
			this.urlMapper.mapRelative('/api/tags/mappings'),
			paging,
		);
	};

	public getTopTags = ({
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

	public saveEntryMappings = ({
		mappings,
	}: {
		mappings: EntryTagMappingContract[];
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative('/api/tags/entry-type-mappings');
		return this.httpClient.put<void>(url, mappings);
	};

	public saveMappings = ({
		mappings,
	}: {
		mappings: TagMappingContract[];
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative('/api/tags/mappings');
		return this.httpClient.put<void>(url, mappings);
	};

	public getDetails = ({ id }: { id: number }): Promise<TagDetailsContract> => {
		return this.httpClient.get<TagDetailsContract>(
			this.urlMapper.mapRelative(`/api/tags/${id}/details`),
		);
	};

	public getTagsByCategories = (): Promise<TagCategoryContract[]> => {
		return this.httpClient.get<TagCategoryContract[]>(
			this.urlMapper.mapRelative('/api/tags/by-categories'),
		);
	};

	public getTagWithArchivedVersions = ({
		id,
	}: {
		id: number;
	}): Promise<EntryWithArchivedVersionsContract<TagApiContract>> => {
		return this.httpClient.get<
			EntryWithArchivedVersionsContract<TagApiContract>
		>(this.urlMapper.mapRelative(`/api/tags/${id}/versions`));
	};

	public getForEdit = ({ id }: { id: number }): Promise<TagForEditContract> => {
		return this.httpClient.get<TagForEditContract>(
			this.urlMapper.mapRelative(`/api/tags/${id}/for-edit`),
		);
	};

	public edit = (
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
					requestVerificationToken: vdb.values.requestToken,
				},
			},
		);
	};
}

export interface TagQueryParams extends CommonQueryParams {
	allowAliases?: boolean;

	categoryName?: string;

	// Comma-separated list of optional fields
	fields?: string;

	sort?: string;
}
