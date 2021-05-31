import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import VenueForApiContract from '@DataContracts/Venue/VenueForApiContract';
import AjaxHelper from '@Helpers/AjaxHelper';
import NameMatchMode from '@Models/NameMatchMode';
import HttpClient from '@Shared/HttpClient';

import BaseRepository from './BaseRepository';

export default class VenueRepository extends BaseRepository {
  constructor(private readonly httpClient: HttpClient) {
    super();
  }

  public createReport = (
    venueId: number,
    reportType: string,
    notes: string,
    versionNumber: number,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      `/api/venues/${venueId}/reports?${AjaxHelper.createUrl({
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
      `/api/venues/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
        notes,
      )}`,
    );
  };

  public getList = (
    query: string,
    nameMatchMode: NameMatchMode,
    maxResults: number,
  ): Promise<PartialFindResultContract<VenueForApiContract>> => {
    var data = {
      query: query,
      maxResults: maxResults,
      nameMatchMode: NameMatchMode[nameMatchMode],
    };

    return this.httpClient.get<PartialFindResultContract<VenueForApiContract>>(
      '/api/venues',
      data,
    );
  };
}
