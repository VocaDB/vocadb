import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ArchivedVenueVersionDetailsContract } from '@/DataContracts/Venue/ArchivedVenueVersionDetailsContract';
import { VenueForApiContract } from '@/DataContracts/Venue/VenueForApiContract';
import { VenueForEditContract } from '@/DataContracts/Venue/VenueForEditContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { AjaxHelper } from '@/Helpers/AjaxHelper';
import { NameMatchMode } from '@/Models/NameMatchMode';
import { BaseRepository } from '@/Repositories/BaseRepository';
import { functions } from '@/Shared/GlobalFunctions';
import { httpClient, HttpClient } from '@/Shared/HttpClient';
import { urlMapper, UrlMapper } from '@/Shared/UrlMapper';
import qs from 'qs';

export class VenueRepository extends BaseRepository {
	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {
		super(urlMapper.baseUrl);
	}

	createReport = ({
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
				`/api/venues/${id}?${qs.stringify({
					hardDelete: hardDelete,
					notes: notes,
				})}`,
			),
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	getList = ({
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

	getDetails = ({ id }: { id: number }): Promise<VenueForApiContract> => {
		return this.httpClient.get<VenueForApiContract>(
			this.urlMapper.mapRelative(`/api/venues/${id}/details`),
		);
	};

	getVenueWithArchivedVersions = ({
		id,
	}: {
		id: number;
	}): Promise<EntryWithArchivedVersionsContract<VenueForApiContract>> => {
		return this.httpClient.get<
			EntryWithArchivedVersionsContract<VenueForApiContract>
		>(this.urlMapper.mapRelative(`/api/venues/${id}/versions`));
	};

	getVersionDetails = ({
		id,
		comparedVersionId,
	}: {
		id: number;
		comparedVersionId?: number;
	}): Promise<ArchivedVenueVersionDetailsContract> => {
		return this.httpClient.get<ArchivedVenueVersionDetailsContract>(
			this.urlMapper.mapRelative(`/api/venues/versions/${id}`),
			{ comparedVersionId: comparedVersionId },
		);
	};

	getOne = ({ id }: { id: number }): Promise<VenueForApiContract> => {
		return this.httpClient.get<VenueForApiContract>(
			this.urlMapper.mapRelative(`/api/venues/${id}`),
		);
	};

	getForEdit = ({ id }: { id: number }): Promise<VenueForEditContract> => {
		return this.httpClient.get<VenueForEditContract>(
			this.urlMapper.mapRelative(`/api/venues/${id}/for-edit`),
		);
	};

	edit = (
		requestToken: string,
		contract: VenueForEditContract,
	): Promise<number> => {
		return this.httpClient.post<number>(
			this.urlMapper.mapRelative(`/api/venues/${contract.id}`),
			contract,
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
				`/api/venues/versions/${archivedVersionId}/update-visibility?${qs.stringify(
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

export const venueRepo = new VenueRepository(httpClient, urlMapper);
