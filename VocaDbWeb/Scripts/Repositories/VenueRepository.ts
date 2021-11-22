import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import VenueForApiContract from '@DataContracts/Venue/VenueForApiContract';
import AjaxHelper from '@Helpers/AjaxHelper';
import NameMatchMode from '@Models/NameMatchMode';
import functions from '@Shared/GlobalFunctions';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

import BaseRepository from './BaseRepository';

export default class VenueRepository extends BaseRepository {
	public constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {
		super(urlMapper.baseUrl);
	}

	public createReport = ({
		entryId: venueId,
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
			`/api/venues/${venueId}/reports?${AjaxHelper.createUrl({
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
				`/api/venues/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
					notes,
				)}`,
			),
		);
	};

	public getList = ({
		query,
		nameMatchMode,
		maxResults,
	}: {
		query: string;
		nameMatchMode: NameMatchMode;
		maxResults: number;
	}): Promise<PartialFindResultContract<VenueForApiContract>> => {
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

	public getDetails = ({
		id,
	}: {
		id: number;
	}): Promise<VenueForApiContract> => {
		return this.httpClient.get<VenueForApiContract>(
			this.urlMapper.mapRelative(`/api/venues/${id}/details`),
		);
	};
}
