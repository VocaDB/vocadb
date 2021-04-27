import AjaxHelper from '../Helpers/AjaxHelper';
import BaseRepository from './BaseRepository';
import functions from '../Shared/GlobalFunctions';
import NameMatchMode from '../Models/NameMatchMode';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import UrlMapper from '../Shared/UrlMapper';
import VenueForApiContract from '../DataContracts/Venue/VenueForApiContract';

export default class VenueRepository extends BaseRepository {
  constructor(private readonly urlMapper: UrlMapper) {
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
    callback?: (result: PartialFindResultContract<VenueForApiContract>) => void,
  ) => {
    var url = functions.mergeUrls(this.baseUrl, '/api/venues');
    var data = {
      query: query,
      maxResults: maxResults,
      nameMatchMode: NameMatchMode[nameMatchMode],
    };

    $.getJSON(url, data, callback);
  };
}
