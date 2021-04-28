import AjaxHelper from '../Helpers/AjaxHelper';
import BaseRepository from './BaseRepository';
import functions from '../Shared/GlobalFunctions';
import NameMatchMode from '../Models/NameMatchMode';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import UrlMapper from '../Shared/UrlMapper';
import VenueForApiContract from '../DataContracts/Venue/VenueForApiContract';
import HttpClient from '../Shared/HttpClient';

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
    callback?: () => void,
  ) => {
    var url = functions.mergeUrls(
      this.baseUrl,
      `/api/venues/${venueId}/reports?${AjaxHelper.createUrl({
        reportType: [reportType],
        notes: [notes],
        versionNumber: [versionNumber],
      })}`,
    );
    $.postJSON(url, callback);
  };

  public delete = (
    id: number,
    notes: string,
    hardDelete: boolean,
    callback?: () => void,
  ) => {
    $.ajax(
      this.urlMapper.mapRelative(
        `/api/venues/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
          notes,
        )}`,
      ),
      { type: 'DELETE', success: callback },
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
