import AjaxHelper from '../Helpers/AjaxHelper';
import BaseRepository from './BaseRepository';
import { CommonQueryParams } from './BaseRepository';
import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import EntryCommentRepository from './EntryCommentRepository';
import EntryTagMappingContract from '../DataContracts/Tag/EntryTagMappingContract';
import EntryType from '../Models/EntryType';
import functions from '../Shared/GlobalFunctions';
import NameMatchMode from '../Models/NameMatchMode';
import PagingProperties from '../DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import TagApiContract from '../DataContracts/Tag/TagApiContract';
import TagBaseContract from '../DataContracts/Tag/TagBaseContract';
import TagMappingContract from '../DataContracts/Tag/TagMappingContract';
import UrlMapper from '../Shared/UrlMapper';
import HttpClient from '../Shared/HttpClient';

export default class TagRepository extends BaseRepository {
  private readonly urlMapper: UrlMapper;

  constructor(
    private readonly httpClient: HttpClient,
    baseUrl: string,
    lang?: ContentLanguagePreference,
  ) {
    super(baseUrl, lang);
    this.urlMapper = new UrlMapper(baseUrl);
  }

  public create = (name: string): Promise<TagBaseContract> => {
    var url = functions.mergeUrls(this.baseUrl, `/api/tags?name=${name}`);
    return this.httpClient.post<TagBaseContract>(url);
  };

  public createReport = (
    tagId: number,
    reportType: string,
    notes: string,
    versionNumber: number,
  ): Promise<void> => {
    var url = functions.mergeUrls(
      this.baseUrl,
      `/api/tags/${tagId}/reports?${AjaxHelper.createUrl({
        reportType: [reportType],
        notes: [notes],
        versionNumber: [versionNumber],
      })}`,
    );
    return this.httpClient.post<void>(url);
  };

  public getById = (
    id: number,
    fields: string,
    lang: string,
  ): Promise<TagApiContract> => {
    var url = functions.mergeUrls(this.baseUrl, `/api/tags/${id}`);
    return this.httpClient.get<TagApiContract>(url, {
      fields: fields || undefined,
      lang: lang,
    });
  };

  public getComments = (): EntryCommentRepository =>
    new EntryCommentRepository(
      this.httpClient,
      new UrlMapper(this.baseUrl),
      '/tags/',
    );

  public getEntryTypeTag = (
    entryType: EntryType,
    subType: string = '',
  ): Promise<TagApiContract> => {
    var url = functions.mergeUrls(
      this.baseUrl,
      `/api/entry-types/${EntryType[entryType]}/${subType}/tag`,
    );
    return this.httpClient.get<TagApiContract>(url, {
      fields: 'Description',
      lang: this.languagePreferenceStr,
    });
  };

  public getList = (
    queryParams: TagQueryParams,
  ): Promise<PartialFindResultContract<TagApiContract>> => {
    var nameMatchMode = queryParams.nameMatchMode || NameMatchMode.Auto;

    var url = functions.mergeUrls(this.baseUrl, '/api/tags');
    var data = {
      start: queryParams.start,
      getTotalCount: queryParams.getTotalCount,
      maxResults: queryParams.maxResults,
      query: queryParams.query,
      fields: queryParams.fields || undefined,
      nameMatchMode: NameMatchMode[nameMatchMode],
      allowAliases: queryParams.allowAliases,
      categoryName: queryParams.categoryName,
      lang: queryParams.lang,
      sort: queryParams.sort,
    };

    return this.httpClient.get<PartialFindResultContract<TagApiContract>>(
      url,
      data,
    );
  };

  public getEntryTagMappings = (): Promise<EntryTagMappingContract[]> => {
    return this.httpClient.get<EntryTagMappingContract[]>(
      this.urlMapper.mapRelative('/api/tags/entry-type-mappings'),
    );
  };

  public getMappings = (
    paging: PagingProperties,
  ): Promise<PartialFindResultContract<TagMappingContract>> => {
    return this.httpClient.get<PartialFindResultContract<TagMappingContract>>(
      this.urlMapper.mapRelative('/api/tags/mappings'),
      paging,
    );
  };

  public getTopTags = (
    lang: string,
    categoryName?: string,
    entryType?: EntryType,
  ): Promise<TagBaseContract[]> => {
    var url = functions.mergeUrls(this.baseUrl, '/api/tags/top');
    var data = {
      lang: lang,
      categoryName: categoryName,
      entryType: entryType || undefined,
    };

    return this.httpClient.get<TagBaseContract[]>(url, data);
  };

  public saveEntryMappings = (
    mappings: EntryTagMappingContract[],
  ): Promise<any> => {
    var url = this.urlMapper.mapRelative('/api/tags/entry-type-mappings');
    return Promise.resolve(AjaxHelper.putJSON(url, mappings));
  };

  public saveMappings = (mappings: TagMappingContract[]): Promise<any> => {
    var url = this.urlMapper.mapRelative('/api/tags/mappings');
    return Promise.resolve(AjaxHelper.putJSON(url, mappings));
  };
}

export interface TagQueryParams extends CommonQueryParams {
  allowAliases?: boolean;

  categoryName?: string;

  // Comma-separated list of optional fields
  fields?: string;

  sort?: string;
}
