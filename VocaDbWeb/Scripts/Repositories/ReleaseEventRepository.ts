import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import ReleaseEventDetailsContract from '@DataContracts/ReleaseEvents/ReleaseEventDetailsContract';
import ReleaseEventForEditContract from '@DataContracts/ReleaseEvents/ReleaseEventForEditContract';
import ReleaseEventSeriesDetailsContract from '@DataContracts/ReleaseEvents/ReleaseEventSeriesDetailsContract';
import ReleaseEventSeriesForApiContract from '@DataContracts/ReleaseEvents/ReleaseEventSeriesForApiContract';
import EntryWithArchivedVersionsContract from '@DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import AjaxHelper from '@Helpers/AjaxHelper';
import NameMatchMode from '@Models/NameMatchMode';
import functions from '@Shared/GlobalFunctions';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

import ReleaseEventSeriesForEditContract from '../DataContracts/ReleaseEvents/ReleaseEventSeriesForEditContract';
import BaseRepository from './BaseRepository';
import { CommonQueryParams } from './BaseRepository';

export default class ReleaseEventRepository extends BaseRepository {
	public constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {
		super(urlMapper.baseUrl);
	}

	public createReport = ({
		entryId: eventId,
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
			`/api/releaseEvents/${eventId}/reports?${AjaxHelper.createUrl({
				reportType: [reportType],
				notes: [notes],
				versionNumber: [versionNumber!],
			})}`,
		);
		return this.httpClient.post<void>(url);
	};

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
				`/api/releaseEvents/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
					notes,
				)}`,
			),
		);
	};

	public deleteSeries = ({
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
				`/api/releaseEventSeries/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
					notes,
				)}`,
			),
		);
	};

	public getList = ({
		queryParams,
	}: {
		queryParams: EventQueryParams;
	}): Promise<PartialFindResultContract<ReleaseEventContract>> => {
		var nameMatchMode = queryParams.nameMatchMode || NameMatchMode.Auto;

		var url = functions.mergeUrls(this.baseUrl, '/api/releaseEvents');
		var data = {
			start: queryParams.start,
			getTotalCount: queryParams.getTotalCount,
			maxResults: queryParams.maxResults,
			query: queryParams.query,
			category: queryParams.category || undefined,
			tagId: queryParams.tagIds,
			childTags: queryParams.childTags,
			fields: queryParams.fields || undefined,
			userCollectionId: queryParams.userCollectionId || undefined,
			artistId: queryParams.artistId || undefined,
			childVoicebanks: queryParams.childVoicebanks || undefined,
			includeMembers: queryParams.includeMembers || undefined,
			status: queryParams.status || undefined,
			afterDate: this.getDate(queryParams.afterDate),
			beforeDate: this.getDate(queryParams.beforeDate),
			nameMatchMode: NameMatchMode[nameMatchMode],
			lang: queryParams.lang,
			sort: queryParams.sort,
			sortDirection: queryParams.sortDirection,
		};

		return this.httpClient.get<PartialFindResultContract<ReleaseEventContract>>(
			url,
			data,
		);
	};

	public getOne = ({ id }: { id: number }): Promise<ReleaseEventContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/releaseEvents/${id}`);
		return this.httpClient.get<ReleaseEventContract>(url);
	};

	public getOneSeries = ({
		id,
	}: {
		id: number;
	}): Promise<ReleaseEventSeriesForApiContract> => {
		return this.httpClient.get<ReleaseEventSeriesForApiContract>(
			this.urlMapper.mapRelative(`/api/releaseEventSeries/${id}`),
		);
	};

	public getOneByName = async ({
		name,
	}: {
		name: string;
	}): Promise<ReleaseEventContract | null> => {
		var url = functions.mergeUrls(
			this.baseUrl,
			`/api/releaseEvents?query=${encodeURIComponent(
				name,
			)}&nameMatchMode=Exact&maxResults=1`,
		);
		const result = await this.httpClient.get<
			PartialFindResultContract<ReleaseEventContract>
		>(url);
		return result && result.items && result.items.length
			? result.items[0]
			: null;
	};

	public getSeriesList = ({
		query,
		nameMatchMode,
		maxResults,
	}: {
		query: string;
		nameMatchMode: NameMatchMode;
		maxResults: number;
	}): Promise<PartialFindResultContract<ReleaseEventSeriesForApiContract>> => {
		var url = functions.mergeUrls(this.baseUrl, '/api/releaseEventSeries');
		var data = {
			query: query,
			maxResults: maxResults,
			nameMatchMode: NameMatchMode[nameMatchMode],
		};

		return this.httpClient.get<
			PartialFindResultContract<ReleaseEventSeriesForApiContract>
		>(url, data);
	};

	public getDetails = ({
		id,
	}: {
		id: number;
	}): Promise<ReleaseEventDetailsContract> => {
		return this.httpClient.get<ReleaseEventDetailsContract>(
			this.urlMapper.mapRelative(`/api/releaseEvents/${id}/details`),
		);
	};

	public getSeriesDetails = ({
		id,
	}: {
		id: number;
	}): Promise<ReleaseEventSeriesDetailsContract> => {
		return this.httpClient.get<ReleaseEventSeriesDetailsContract>(
			this.urlMapper.mapRelative(`/api/releaseEventSeries/${id}/details`),
		);
	};

	public getReleaseEventWithArchivedVersions = ({
		id,
	}: {
		id: number;
	}): Promise<EntryWithArchivedVersionsContract<ReleaseEventContract>> => {
		return this.httpClient.get<
			EntryWithArchivedVersionsContract<ReleaseEventContract>
		>(this.urlMapper.mapRelative(`/api/releaseEvents/${id}/versions`));
	};

	public getReleaseEventSeriesWithArchivedVersions = ({
		id,
	}: {
		id: number;
	}): Promise<
		EntryWithArchivedVersionsContract<ReleaseEventSeriesForApiContract>
	> => {
		return this.httpClient.get<
			EntryWithArchivedVersionsContract<ReleaseEventSeriesForApiContract>
		>(this.urlMapper.mapRelative(`/api/releaseEventSeries/${id}/versions`));
	};

	public getForEdit = ({
		id,
	}: {
		id: number;
	}): Promise<ReleaseEventForEditContract> => {
		return this.httpClient.get<ReleaseEventForEditContract>(
			this.urlMapper.mapRelative(`/api/releaseEvents/${id}/for-edit`),
		);
	};

	public edit = (
		contract: ReleaseEventForEditContract,
		pictureUpload: File | undefined,
	): Promise<number> => {
		const formData = new FormData();
		formData.append('contract', JSON.stringify(contract));

		if (pictureUpload) formData.append('pictureUpload', pictureUpload);

		return this.httpClient.post<number>(
			this.urlMapper.mapRelative(`/api/releaseEvents/${contract.id}`),
			formData,
			{
				headers: {
					'Content-Type': 'multipart/form-data',
					requestVerificationToken: vdb.values.requestToken,
				},
			},
		);
	};

	public getSeriesForEdit = ({
		id,
	}: {
		id: number;
	}): Promise<ReleaseEventSeriesForEditContract> => {
		return this.httpClient.get<ReleaseEventSeriesForEditContract>(
			this.urlMapper.mapRelative(`/api/releaseEventSeries/${id}/for-edit`),
		);
	};

	public editSeries = (
		contract: ReleaseEventSeriesForEditContract,
		pictureUpload: File | undefined,
	): Promise<number> => {
		const formData = new FormData();
		formData.append('contract', JSON.stringify(contract));

		if (pictureUpload) formData.append('pictureUpload', pictureUpload);

		return this.httpClient.post<number>(
			this.urlMapper.mapRelative(`/api/releaseEventSeries/${contract.id}`),
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

export interface EventQueryParams extends CommonQueryParams {
	afterDate?: Date;

	artistId?: number[];

	beforeDate?: Date;

	category?: string;

	childTags: boolean;

	childVoicebanks?: boolean;

	// Comma-separated list of optional fields
	fields?: string;

	includeMembers?: boolean;

	sort?: string;

	sortDirection?: 'Ascending' | 'Descending';

	status?: string;

	tagIds: number[];

	userCollectionId?: number;
}
