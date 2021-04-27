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

// Repository for managing artists and related objects.
// Corresponds to the ArtistController class.
export default class ArtistRepository
  extends BaseRepository
  implements ICommentRepository {
  public createComment = (
    artistId: number,
    contract: CommentContract,
    callback: (contract: CommentContract) => void,
  ) => {
    $.postJSON(
      this.urlMapper.mapRelative(`/api/artists/${artistId}/comments`),
      contract,
      callback,
      'json',
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

  public getComments = (
    artistId: number,
    callback: (contract: CommentContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative(`/api/artists/${artistId}/comments`),
      callback,
    );
  };

  public getForEdit = (
    id: number,
    callback: (result: ArtistForEditContract) => void,
  ) => {
    var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}/for-edit`);
    $.getJSON(url, callback);
  };

  public getOne = (id: number, callback: (result: ArtistContract) => void) => {
    var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}`);
    $.getJSON(
      url,
      { fields: 'AdditionalNames', lang: this.languagePreferenceStr },
      callback,
    );
  };

  public getOneWithComponents = (
    id: number,
    fields: string,
    callback: (result: ArtistApiContract) => void,
  ) => {
    var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}`);
    $.getJSON(
      url,
      { fields: fields, lang: this.languagePreferenceStr },
      callback,
    );
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
    callback,
  ) => {
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

    $.getJSON(url, data, callback);
  };

  public getTagSuggestions = (
    artistId: number,
    callback: (contract: TagUsageForApiContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative(`/api/artists/${artistId}/tagSuggestions`),
      callback,
    );
  };

  // Maps a relative URL to an absolute one.
  private mapUrl: (relative: string) => string;

  private urlMapper: UrlMapper;

  public updateComment = (
    commentId: number,
    contract: CommentContract,
    callback?: () => void,
  ) => {
    $.postJSON(
      this.urlMapper.mapRelative(`/api/artists/comments/${commentId}`),
      contract,
      callback,
      'json',
    );
  };

  constructor(baseUrl: string, lang?: ContentLanguagePreference) {
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
