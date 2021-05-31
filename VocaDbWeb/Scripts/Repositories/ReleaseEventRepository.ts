import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import ReleaseEventSeriesForApiContract from '@DataContracts/ReleaseEvents/ReleaseEventSeriesForApiContract';
import AjaxHelper from '@Helpers/AjaxHelper';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import NameMatchMode from '@Models/NameMatchMode';
import HttpClient from '@Shared/HttpClient';

import BaseRepository from './BaseRepository';
import { CommonQueryParams } from './BaseRepository';

export default class ReleaseEventRepository extends BaseRepository {
	constructor(private readonly httpClient: HttpClient) {
		super();
	}

	public createReport = (
		eventId: number,
		reportType: string,
		notes: string,
		versionNumber: number,
	): Promise<void> => {
		return this.httpClient.post<void>(
			`/api/releaseEvents/${eventId}/reports?${AjaxHelper.createUrl({
				reportType: [reportType],
				notes: [notes],
				versionNumber: [versionNumber],
			})}`,
		);
	};

	public delete = (
		id: number,
		notes: string,
		hardDelete: boolean,
	): Promise<void> => {
		return this.httpClient.delete<void>(
			`/api/releaseEvents/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
				notes,
			)}`,
		);
	};

	public deleteSeries = (
		id: number,
		notes: string,
		hardDelete: boolean,
	): Promise<void> => {
		return this.httpClient.delete<void>(
			`/api/releaseEventSeries/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
				notes,
			)}`,
		);
	};

	public getList = (
		queryParams: EventQueryParams,
	): Promise<PartialFindResultContract<ReleaseEventContract>> => {
		var nameMatchMode = queryParams.nameMatchMode || NameMatchMode.Auto;

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
			'/api/releaseEvents',
			data,
		);
	};

	public getOne = (id: number): Promise<ReleaseEventContract> => {
		return this.httpClient.get<ReleaseEventContract>(
			`/api/releaseEvents/${id}`,
		);
	};

	public getOneByName = async (
		name: string,
	): Promise<ReleaseEventContract | null> => {
		const result = await this.httpClient.get<
			PartialFindResultContract<ReleaseEventContract>
		>(
			`/api/releaseEvents?query=${encodeURIComponent(
				name,
			)}&nameMatchMode=Exact&maxResults=1`,
		);
		return result && result.items && result.items.length
			? result.items[0]
			: null;
	};

	public getSeriesList = (
		query: string,
		nameMatchMode: NameMatchMode,
		maxResults: number,
	): Promise<PartialFindResultContract<ReleaseEventSeriesForApiContract>> => {
		var data = {
			query: query,
			maxResults: maxResults,
			nameMatchMode: NameMatchMode[nameMatchMode],
		};

		return this.httpClient.get<
			PartialFindResultContract<ReleaseEventSeriesForApiContract>
		>('/api/releaseEventSeries', data);
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
