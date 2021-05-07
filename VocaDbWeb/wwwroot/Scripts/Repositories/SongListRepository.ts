import AdvancedSearchFilter from '../ViewModels/Search/AdvancedSearchFilter';
import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import EntryCommentRepository from './EntryCommentRepository';
import PagingProperties from '../DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import SongInListContract from '../DataContracts/Song/SongInListContract';
import SongListContract from '../DataContracts/Song/SongListContract';
import SongListForEditContract from '../DataContracts/Song/SongListForEditContract';
import { SongOptionalFields } from '../Models/EntryOptionalFields';
import UrlMapper from '../Shared/UrlMapper';
import HttpClient from '../Shared/HttpClient';

export default class SongListRepository {
  constructor(
    private readonly httpClient: HttpClient,
    private readonly urlMapper: UrlMapper,
  ) {}

  public delete = (
    id: number,
    notes: string,
    hardDelete: boolean,
  ): Promise<void> => {
    return this.httpClient.delete<void>(
      this.urlMapper.mapRelative(
        `/api/songLists/${id}?hardDelete=${hardDelete}&notes=${encodeURIComponent(
          notes,
        )}`,
      ),
    );
  };

  public getComments = (): EntryCommentRepository =>
    new EntryCommentRepository(this.httpClient, this.urlMapper, '/songLists/');

  public getFeatured = (
    query: string,
    category: string,
    paging: PagingProperties,
    tagIds: number[],
    fields: string,
    sort: string,
  ): Promise<PartialFindResultContract<SongListContract>> => {
    var url = this.urlMapper.mapRelative('/api/songLists/featured');
    return this.httpClient.get<PartialFindResultContract<SongListContract>>(
      url,
      {
        query: query,
        featuredCategory: category,
        start: paging.start,
        getTotalCount: paging.getTotalCount,
        maxResults: paging.maxEntries,
        tagId: tagIds,
        fields: fields,
        sort: sort,
      },
    );
  };

  public getForEdit = (id: number): Promise<SongListForEditContract> => {
    var url = this.urlMapper.mapRelative(`/api/songLists/${id}/for-edit`);
    return this.httpClient.get<SongListForEditContract>(url);
  };

  public getSongs = (
    listId: number,
    query: string,
    songTypes: string,
    tagIds: number[],
    childTags: boolean,
    artistIds: number[],
    artistParticipationStatus: string,
    childVoicebanks: boolean,
    advancedFilters: AdvancedSearchFilter[],
    pvServices: string,
    paging: PagingProperties,
    fields: SongOptionalFields,
    sort: string,
    lang: ContentLanguagePreference,
  ): Promise<PartialFindResultContract<SongInListContract>> => {
    var url = this.urlMapper.mapRelative(`/api/songLists/${listId}/songs`);
    var data = {
      query: query,
      songTypes: songTypes,
      tagId: tagIds,
      childTags: childTags,
      artistId: artistIds,
      artistParticipationStatus: artistParticipationStatus,
      childVoicebanks: childVoicebanks,
      advancedFilters: advancedFilters,
      pvServices: pvServices,
      start: paging.start,
      getTotalCount: paging.getTotalCount,
      maxResults: paging.maxEntries,
      fields: fields.fields,
      lang: ContentLanguagePreference[lang],
      sort: sort,
    };

    return this.httpClient.get<PartialFindResultContract<SongInListContract>>(
      url,
      data,
    );
  };
}
