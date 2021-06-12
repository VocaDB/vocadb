import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import ReleaseEventSeriesForApiContract from '@DataContracts/ReleaseEvents/ReleaseEventSeriesForApiContract';
import AjaxHelper from '@Helpers/AjaxHelper';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import NameMatchMode from '@Models/NameMatchMode';
import functions from '@Shared/GlobalFunctions';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

import BaseRepository from './BaseRepository';
import { CommonQueryParams } from './BaseRepository';

export default class ReleaseEventRepository extends BaseRepository {
	public constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {
		super(urlMapper.baseUrl);
	}

	public createReport = (
		eventId: number,
		reportType: string,
		notes: string,
		versionNumber: number,
	): Promise<void> => {
		var url = functions.mergeUrls(
			this.baseUrl,
			`/api/releaseEvents/${eventId}/reports?${AjaxHelper.createUrl({
				reportType: [reportType],
				notes: [notes],
				versionNumber: [versionNumber],
			})}`,
		);
		return this.httpClient.post<void>(url);
	};

	public delete = (
		id: number,
		notes: string,
		hardDelete: boolean,
	): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(
				`/api/releaseEvents/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
					notes,
				)}`,
			),
		);
	};

	public deleteSeries = (
		id: number,
		notes: string,
		hardDelete: boolean,
	): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(
				`/api/releaseEventSeries/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
					notes,
				)}`,
			),
		);
	};

	public getList = (
		queryParams: EventQueryParams,
	): Promise<PartialFindResultContract<ReleaseEventContract>> => {
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
			lang: queryParams.lang
				? ContentLanguagePreference[queryParams.lang]
				: undefined,
			sort: queryParams.sort,
		};

		return this.httpClient.get<PartialFindResultContract<ReleaseEventContract>>(
			url,
			data,
		);
	};

	public getOne = (id: number): Promise<ReleaseEventContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/releaseEvents/${id}`);
		return this.httpClient.get<ReleaseEventContract>(url);
	};

	public getOneByName = async (
		name: string,
	): Promise<ReleaseEventContract | null> => {
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

	public getSeriesList = (
		query: string,
		nameMatchMode: NameMatchMode,
		maxResults: number,
	): Promise<PartialFindResultContract<ReleaseEventSeriesForApiContract>> => {
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

	status?: string;

	tagIds: number[];

	userCollectionId?: number;
}
