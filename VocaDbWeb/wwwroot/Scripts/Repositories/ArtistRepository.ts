import AdvancedSearchFilter from '../ViewModels/Search/AdvancedSearchFilter';
import ArtistApiContract from '../DataContracts/Artist/ArtistApiContract';
import ArtistContract from '../DataContracts/Artist/ArtistContract';
import ArtistForEditContract from '../DataContracts/Artist/ArtistForEditContract';
import BaseRepository from './BaseRepository';
import CommentContract from '../DataContracts/CommentContract';
import { CommonQueryParams } from './BaseRepository';
import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import DuplicateEntryResultContract from '../DataContracts/DuplicateEntryResultContract';
import functions from '../Shared/GlobalFunctions';
import ICommentRepository from './ICommentRepository';
import PagingProperties from '../DataContracts/PagingPropertiesContract';
import TagUsageForApiContract from '../DataContracts/Tag/TagUsageForApiContract';
import UrlMapper from '../Shared/UrlMapper';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import HttpClient from '../Shared/HttpClient';

// Repository for managing artists and related objects.
// Corresponds to the ArtistController class.
export default class ArtistRepository
  extends BaseRepository
  implements ICommentRepository {
  public createComment = (
    artistId: number,
    contract: CommentContract,
  ): Promise<CommentContract> => {
    return this.httpClient.post<CommentContract>(
      this.urlMapper.mapRelative(`/api/artists/${artistId}/comments`),
      contract,
    );
  };

  public createReport = (
    artistId: number,
    reportType: string,
    notes: string,
    versionNumber: number,
    callback?: () => void,
  ) => {
    $.post(
      this.urlMapper.mapRelative('/Artist/CreateReport'),
      {
        reportType: reportType,
        notes: notes,
        artistId: artistId,
        versionNumber: versionNumber,
      },
      callback,
      'json',
    );
  };

  public deleteComment = (commentId: number, callback?: () => void) => {
    $.ajax(this.urlMapper.mapRelative(`/api/artists/comments/${commentId}`), {
      type: 'DELETE',
      success: callback,
    });
  };

  public findDuplicate: (
    params,
    callback: (result: DuplicateEntryResultContract[]) => void,
  ) => void;

  public getComments = (artistId: number): Promise<CommentContract[]> => {
    return this.httpClient.get<CommentContract[]>(
      this.urlMapper.mapRelative(`/api/artists/${artistId}/comments`),
    );
  };

  public getForEdit = (id: number): Promise<ArtistForEditContract> => {
    var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}/for-edit`);
    return this.httpClient.get<ArtistForEditContract>(url);
  };

  public getOne = (id: number): Promise<ArtistContract> => {
    var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}`);
    return this.httpClient.get<ArtistContract>(url, {
      fields: 'AdditionalNames',
      lang: this.languagePreferenceStr,
    });
  };

  public getOneWithComponents = (
    id: number,
    fields: string,
  ): Promise<ArtistApiContract> => {
    var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}`);
    return this.httpClient.get<ArtistApiContract>(url, {
      fields: fields,
      lang: this.languagePreferenceStr,
    });
  };

  public getList = (
    paging: PagingProperties,
    lang: string,
    query: string,
    sort: string,
    artistTypes: string,
    allowBaseVoicebanks: boolean,
    tags: number[],
    childTags: boolean,
    followedByUserId: number,
    fields: string,
    status: string,
    advancedFilters: AdvancedSearchFilter[],
  ): Promise<PartialFindResultContract<ArtistContract>> => {
    var url = functions.mergeUrls(this.baseUrl, '/api/artists');
    var data = {
      start: paging.start,
      getTotalCount: paging.getTotalCount,
      maxResults: paging.maxEntries,
      query: query,
      fields: fields,
      lang: lang,
      nameMatchMode: 'Auto',
      sort: sort,
      artistTypes: artistTypes,
      allowBaseVoicebanks: allowBaseVoicebanks,
      tagId: tags,
      childTags: childTags,
      followedByUserId: followedByUserId,
      status: status,
      advancedFilters: advancedFilters,
    };

    return this.httpClient.get<PartialFindResultContract<ArtistContract>>(
      url,
      data,
    );
  };

  public getTagSuggestions = (
    artistId: number,
  ): Promise<TagUsageForApiContract[]> => {
    return this.httpClient.get<TagUsageForApiContract[]>(
      this.urlMapper.mapRelative(`/api/artists/${artistId}/tagSuggestions`),
    );
  };

  // Maps a relative URL to an absolute one.
  private mapUrl: (relative: string) => string;

  private urlMapper: UrlMapper;

  public updateComment = (
    commentId: number,
    contract: CommentContract,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      this.urlMapper.mapRelative(`/api/artists/comments/${commentId}`),
      contract,
    );
  };

  constructor(
    private readonly httpClient: HttpClient,
    baseUrl: string,
    lang?: ContentLanguagePreference,
  ) {
    super(baseUrl, lang);

    this.urlMapper = new UrlMapper(baseUrl);

    this.mapUrl = (relative: string) => {
      return `${functions.mergeUrls(baseUrl, '/Artist')}${relative}`;
    };

    this.findDuplicate = (
      params,
      callback: (result: DuplicateEntryResultContract[]) => void,
    ) => {
      $.post(this.mapUrl('/FindDuplicate'), params, callback);
    };
  }
}

export interface ArtistQueryParams extends CommonQueryParams {
  artistTypes: string;
}
