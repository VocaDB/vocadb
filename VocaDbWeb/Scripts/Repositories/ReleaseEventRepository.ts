import { EntryWithTagUsagesForApiContract } from '@/DataContracts/Base/EntryWithTagUsagesForApiContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ArchivedEventSeriesVersionDetailsContract } from '@/DataContracts/ReleaseEvents/ArchivedEventSeriesVersionDetailsContract';
import { ArchivedEventVersionDetailsContract } from '@/DataContracts/ReleaseEvents/ArchivedEventVersionDetailsContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { ReleaseEventDetailsContract } from '@/DataContracts/ReleaseEvents/ReleaseEventDetailsContract';
import { ReleaseEventForEditContract } from '@/DataContracts/ReleaseEvents/ReleaseEventForEditContract';
import { ReleaseEventSeriesDetailsContract } from '@/DataContracts/ReleaseEvents/ReleaseEventSeriesDetailsContract';
import { ReleaseEventSeriesForApiContract } from '@/DataContracts/ReleaseEvents/ReleaseEventSeriesForApiContract';
import { ReleaseEventSeriesForEditContract } from '@/DataContracts/ReleaseEvents/ReleaseEventSeriesForEditContract';
import { ReleaseEventSeriesWithEventsContract } from '@/DataContracts/ReleaseEvents/ReleaseEventSeriesWithEventsContract';
import { VenueForApiContract } from '@/DataContracts/Venue/VenueForApiContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { AjaxHelper } from '@/Helpers/AjaxHelper';
import { NameMatchMode } from '@/Models/NameMatchMode';
import {
	BaseRepository,
	CommonQueryParams,
} from '@/Repositories/BaseRepository';
import { functions } from '@/Shared/GlobalFunctions';
import { httpClient, HttpClient } from '@/Shared/HttpClient';
import { urlMapper, UrlMapper } from '@/Shared/UrlMapper';
import qs from 'qs';

export enum ReleaseEventOptionalField {
	'AdditionalNames' = 'AdditionalNames',
	'Artists' = 'Artists',
	'Description' = 'Description',
	'MainPicture' = 'MainPicture',
	'Names' = 'Names',
	'Series' = 'Series',
	'SongList' = 'SongList',
	'Tags' = 'Tags',
	'Venue' = 'Venue',
	'WebLinks' = 'WebLinks',
	'PVs' = 'PVs',
}

export class ReleaseEventRepository extends BaseRepository {
	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {
		super(urlMapper.baseUrl);
	}

	createReport = ({
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

	delete = (
		requestToken: string,
		{
			id,
			notes,
			hardDelete,
		}: {
			id: number;
			notes: string;
			hardDelete: boolean;
		},
	): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(
				`/api/releaseEvents/${id}?${qs.stringify({
					hardDelete: hardDelete,
					notes: notes,
				})}`,
			),
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	deleteSeries = (
		requestToken: string,
		{
			id,
			notes,
			hardDelete,
		}: {
			id: number;
			notes: string;
			hardDelete: boolean;
		},
	): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(
				`/api/releaseEventSeries/${id}?${qs.stringify({
					hardDelete: hardDelete,
					notes: notes,
				})}`,
			),
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	getList = ({
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
			fields: queryParams.fields?.join(','),
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

	getTagUsages = ({
		eventId,
	}: {
		eventId: number;
	}): Promise<EntryWithTagUsagesForApiContract> => {
		return this.httpClient.get<EntryWithTagUsagesForApiContract>(
			this.urlMapper.mapRelative(`/api/releaseEvents/${eventId}/tagUsages`),
		);
	};

	getOne = ({
		id,
		fields,
	}: {
		id: number;
		fields?: ReleaseEventOptionalField[];
	}): Promise<ReleaseEventContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/releaseEvents/${id}`);
		return this.httpClient.get<ReleaseEventContract>(url, {
			fields: fields?.join(','),
		});
	};

	getOneSeries = ({
		id,
	}: {
		id: number;
	}): Promise<ReleaseEventSeriesForApiContract> => {
		return this.httpClient.get<ReleaseEventSeriesForApiContract>(
			this.urlMapper.mapRelative(`/api/releaseEventSeries/${id}`),
		);
	};

	getOneByName = async ({
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

	getSeriesList = ({
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

	getDetails = ({
		id,
	}: {
		id: number;
	}): Promise<ReleaseEventDetailsContract> => {
		return this.httpClient.get<ReleaseEventDetailsContract>(
			this.urlMapper.mapRelative(`/api/releaseEvents/${id}/details`),
		);
	};

	getSeriesDetails = ({
		id,
	}: {
		id: number;
	}): Promise<ReleaseEventSeriesDetailsContract> => {
		return this.httpClient.get<ReleaseEventSeriesDetailsContract>(
			this.urlMapper.mapRelative(`/api/releaseEventSeries/${id}/details`),
		);
	};

	getReleaseEventWithArchivedVersions = ({
		id,
	}: {
		id: number;
	}): Promise<EntryWithArchivedVersionsContract<ReleaseEventContract>> => {
		return this.httpClient.get<
			EntryWithArchivedVersionsContract<ReleaseEventContract>
		>(this.urlMapper.mapRelative(`/api/releaseEvents/${id}/versions`));
	};

	getVersionDetails = ({
		id,
		comparedVersionId,
	}: {
		id: number;
		comparedVersionId?: number;
	}): Promise<ArchivedEventVersionDetailsContract> => {
		return this.httpClient.get<ArchivedEventVersionDetailsContract>(
			this.urlMapper.mapRelative(`/api/releaseEvents/versions/${id}`),
			{ comparedVersionId: comparedVersionId },
		);
	};

	getReleaseEventSeriesWithArchivedVersions = ({
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

	getSeriesVersionDetails = ({
		id,
		comparedVersionId,
	}: {
		id: number;
		comparedVersionId?: number;
	}): Promise<ArchivedEventSeriesVersionDetailsContract> => {
		return this.httpClient.get<ArchivedEventSeriesVersionDetailsContract>(
			this.urlMapper.mapRelative(`/api/releaseEventSeries/versions/${id}`),
			{ comparedVersionId: comparedVersionId },
		);
	};

	getForEdit = ({
		id,
	}: {
		id: number;
	}): Promise<ReleaseEventForEditContract> => {
		return this.httpClient.get<ReleaseEventForEditContract>(
			this.urlMapper.mapRelative(`/api/releaseEvents/${id}/for-edit`),
		);
	};

	edit = (
		requestToken: string,
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
					requestVerificationToken: requestToken,
				},
			},
		);
	};

	getSeriesForEdit = ({
		id,
	}: {
		id: number;
	}): Promise<ReleaseEventSeriesForEditContract> => {
		return this.httpClient.get<ReleaseEventSeriesForEditContract>(
			this.urlMapper.mapRelative(`/api/releaseEventSeries/${id}/for-edit`),
		);
	};

	editSeries = (
		requestToken: string,
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
					requestVerificationToken: requestToken,
				},
			},
		);
	};

	getByDate = (): Promise<ReleaseEventContract[]> => {
		return this.httpClient.get<ReleaseEventContract[]>(
			this.urlMapper.mapRelative('/api/releaseEvents/by-date'),
		);
	};

	getBySeries = (): Promise<ReleaseEventSeriesWithEventsContract[]> => {
		return this.httpClient.get<ReleaseEventSeriesWithEventsContract[]>(
			this.urlMapper.mapRelative('/api/releaseEvents/by-series'),
		);
	};

	getByVenue = (): Promise<VenueForApiContract[]> => {
		return this.httpClient.get<VenueForApiContract[]>(
			this.urlMapper.mapRelative('/api/releaseEvents/by-venue'),
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
				`/api/releaseEvents/versions/${archivedVersionId}/update-visibility?${qs.stringify(
					{
						hidden: hidden,
					},
				)}`,
			),
			undefined,
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	updateSeriesVersionVisibility = (
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
				`/api/releaseEventSeries/versions/${archivedVersionId}/update-visibility?${qs.stringify(
					{ hidden: hidden },
				)}`,
			),
			undefined,
			{ headers: { requestVerificationToken: requestToken } },
		);
	};
}

export interface EventQueryParams extends CommonQueryParams {
	afterDate?: Date;
	artistId?: number[];
	beforeDate?: Date;
	category?: string;
	childTags?: boolean;
	childVoicebanks?: boolean;
	// Comma-separated list of optional fields
	fields?: ReleaseEventOptionalField[];
	includeMembers?: boolean;
	sort?: string;
	sortDirection?: 'Ascending' | 'Descending';
	status?: string;
	tagIds?: number[];
	userCollectionId?: number;
}

export const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);
