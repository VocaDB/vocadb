import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import VenueForApiContract from '@DataContracts/Venue/VenueForApiContract';
import AjaxHelper from '@Helpers/AjaxHelper';
import NameMatchMode from '@Models/NameMatchMode';
import functions from '@Shared/GlobalFunctions';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

import BaseRepository from './BaseRepository';

export default class VenueRepository extends BaseRepository {
	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {
		super(urlMapper.baseUrl);
	}

	public createReport = (
		venueId: number,
		reportType: string,
		notes: string,
		versionNumber: number,
	): Promise<void> => {
		var url = functions.mergeUrls(
			this.baseUrl,
			`/api/venues/${venueId}/reports?${AjaxHelper.createUrl({
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
				`/api/venues/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
					notes,
				)}`,
			),
		);
	};

	public getList = (
		query: string,
		nameMatchMode: NameMatchMode,
		maxResults: number,
	): Promise<PartialFindResultContract<VenueForApiContract>> => {
		var url = functions.mergeUrls(this.baseUrl, '/api/venues');
		var data = {
			query: query,
			maxResults: maxResults,
			nameMatchMode: NameMatchMode[nameMatchMode],
		};

		return this.httpClient.get<PartialFindResultContract<VenueForApiContract>>(
			url,
			data,
		);
	};
}
