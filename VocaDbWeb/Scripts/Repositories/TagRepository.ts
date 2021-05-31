import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import EntryTagMappingContract from '@DataContracts/Tag/EntryTagMappingContract';
import TagApiContract from '@DataContracts/Tag/TagApiContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import TagMappingContract from '@DataContracts/Tag/TagMappingContract';
import AjaxHelper from '@Helpers/AjaxHelper';
import EntryType from '@Models/EntryType';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import NameMatchMode from '@Models/NameMatchMode';
import HttpClient from '@Shared/HttpClient';

import BaseRepository from './BaseRepository';
import { CommonQueryParams } from './BaseRepository';
import EntryCommentRepository from './EntryCommentRepository';

export default class TagRepository extends BaseRepository {
  constructor(private readonly httpClient: HttpClient) {
    super();
  }

  public create = (name: string): Promise<TagBaseContract> => {
    return this.httpClient.post<TagBaseContract>(`/api/tags?name=${name}`);
  };

  public createReport = (
    tagId: number,
    reportType: string,
    notes: string,
    versionNumber: number,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      `/api/tags/${tagId}/reports?${AjaxHelper.createUrl({
        reportType: [reportType],
        notes: [notes],
        versionNumber: [versionNumber],
      })}`,
    );
  };

  public getById = (
    id: number,
    fields: string,
    lang: ContentLanguagePreference,
  ): Promise<TagApiContract> => {
    return this.httpClient.get<TagApiContract>(`/api/tags/${id}`, {
      fields: fields || undefined,
      lang: ContentLanguagePreference[lang],
    });
  };

  public getComments = (): EntryCommentRepository =>
    new EntryCommentRepository(this.httpClient, '/tags/');

  public getEntryTypeTag = (
    entryType: EntryType,
    subType: string,
    lang: ContentLanguagePreference,
  ): Promise<TagApiContract> => {
    return this.httpClient.get<TagApiContract>(
      `/api/entry-types/${EntryType[entryType]}/${subType}/tag`,
      {
        fields: 'Description',
        lang: ContentLanguagePreference[lang],
      },
    );
  };

  public getList = (
    queryParams: TagQueryParams,
  ): Promise<PartialFindResultContract<TagApiContract>> => {
    var nameMatchMode = queryParams.nameMatchMode || NameMatchMode.Auto;

    var data = {
      start: queryParams.start,
      getTotalCount: queryParams.getTotalCount,
      maxResults: queryParams.maxResults,
      query: queryParams.query,
      fields: queryParams.fields || undefined,
      nameMatchMode: NameMatchMode[nameMatchMode],
      allowAliases: queryParams.allowAliases,
      categoryName: queryParams.categoryName,
      lang: queryParams.lang
        ? ContentLanguagePreference[queryParams.lang]
        : undefined,
      sort: queryParams.sort,
    };

    return this.httpClient.get<PartialFindResultContract<TagApiContract>>(
      '/api/tags',
      data,
    );
  };

  public getEntryTagMappings = (): Promise<EntryTagMappingContract[]> => {
    return this.httpClient.get<EntryTagMappingContract[]>(
      '/api/tags/entry-type-mappings',
    );
  };

  public getMappings = (
    paging: PagingProperties,
  ): Promise<PartialFindResultContract<TagMappingContract>> => {
    return this.httpClient.get<PartialFindResultContract<TagMappingContract>>(
      '/api/tags/mappings',
      paging,
    );
  };

  public getTopTags = (
    lang: ContentLanguagePreference,
    categoryName?: string,
    entryType?: EntryType,
  ): Promise<TagBaseContract[]> => {
    var data = {
      lang: ContentLanguagePreference[lang],
      categoryName: categoryName,
      entryType: entryType || undefined,
    };

    return this.httpClient.get<TagBaseContract[]>('/api/tags/top', data);
  };

  public saveEntryMappings = (
    mappings: EntryTagMappingContract[],
  ): Promise<void> => {
    return this.httpClient.put<void>('/api/tags/entry-type-mappings', mappings);
  };

  public saveMappings = (mappings: TagMappingContract[]): Promise<void> => {
    return this.httpClient.put<void>('/api/tags/mappings', mappings);
  };
}

export interface TagQueryParams extends CommonQueryParams {
  allowAliases?: boolean;

  categoryName?: string;

  // Comma-separated list of optional fields
  fields?: string;

  sort?: string;
}
