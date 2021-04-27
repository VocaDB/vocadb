import AjaxHelper from '../Helpers/AjaxHelper';
import BaseRepository from './BaseRepository';
import { CommonQueryParams } from './BaseRepository';
import functions from '../Shared/GlobalFunctions';
import NameMatchMode from '../Models/NameMatchMode';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import ReleaseEventContract from '../DataContracts/ReleaseEvents/ReleaseEventContract';
import ReleaseEventSeriesForApiContract from '../DataContracts/ReleaseEvents/ReleaseEventSeriesForApiContract';
import UrlMapper from '../Shared/UrlMapper';

export default class ReleaseEventRepository extends BaseRepository {
  constructor(private readonly urlMapper: UrlMapper) {
    super(urlMapper.baseUrl);
  }

  public createReport = (
    eventId: number,
    reportType: string,
    notes: string,
    versionNumber: number,
    callback?: () => void,
  ) => {
    var url = functions.mergeUrls(
      this.baseUrl,
      `/api/releaseEvents/${eventId}/reports?${AjaxHelper.createUrl({
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
        `/api/releaseEvents/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
          notes,
        )}`,
      ),
      { type: 'DELETE', success: callback },
    );
  };

  public deleteSeries = (
    id: number,
    notes: string,
    hardDelete: boolean,
    callback?: () => void,
  ) => {
    $.ajax(
      this.urlMapper.mapRelative(
        `/api/releaseEventSeries/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
          notes,
        )}`,
      ),
      { type: 'DELETE', success: callback },
    );
  };

  public getList = (
    queryParams: EventQueryParams,
    callback?: (
      result: PartialFindResultContract<ReleaseEventContract>,
    ) => void,
  ) => {
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
    };

    $.getJSON(url, data, callback);
  };

  public getOne = (
    id: number,
    callback?: (result: ReleaseEventContract) => void,
  ) => {
    var url = functions.mergeUrls(this.baseUrl, `/api/releaseEvents/${id}`);
    $.getJSON(url, {}, callback);
  };

  public getOneByName = (
    name: string,
    callback?: (result: ReleaseEventContract) => void,
  ) => {
    var url = functions.mergeUrls(
      this.baseUrl,
      `/api/releaseEvents?query=${encodeURIComponent(
        name,
      )}&nameMatchMode=Exact&maxResults=1`,
    );
    $.getJSON(url, {}, (result) =>
      callback(
        result && result.items && result.items.length ? result.items[0] : null,
      ),
    );
  };

  public getSeriesList = (
    query: string,
    nameMatchMode: NameMatchMode,
    maxResults: number,
    callback?: (
      result: PartialFindResultContract<ReleaseEventSeriesForApiContract>,
    ) => void,
  ) => {
    var url = functions.mergeUrls(this.baseUrl, '/api/releaseEventSeries');
    var data = {
      query: query,
      maxResults: maxResults,
      nameMatchMode: NameMatchMode[nameMatchMode],
    };

    $.getJSON(url, data, callback);
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
