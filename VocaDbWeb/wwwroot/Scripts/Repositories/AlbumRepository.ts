import AdvancedSearchFilter from '../ViewModels/Search/AdvancedSearchFilter';
import AlbumContract from '../DataContracts/Album/AlbumContract';
import AlbumForApiContract from '../DataContracts/Album/AlbumForApiContract';
import AlbumForEditContract from '../DataContracts/Album/AlbumForEditContract';
import AlbumForUserForApiContract from '../DataContracts/User/AlbumForUserForApiContract';
import AlbumReviewContract from '../DataContracts/Album/AlbumReviewContract';
import ArtistContract from '../DataContracts/Artist/ArtistContract';
import BaseRepository from './BaseRepository';
import CommentContract from '../DataContracts/CommentContract';
import { CommonQueryParams } from './BaseRepository';
import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import DuplicateEntryResultContract from '../DataContracts/DuplicateEntryResultContract';
import functions from '../Shared/GlobalFunctions';
import PagingProperties from '../DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import TagUsageForApiContract from '../DataContracts/Tag/TagUsageForApiContract';
import UrlMapper from '../Shared/UrlMapper';

// Repository for managing albums and related objects.
// Corresponds to the AlbumController class.
export default class AlbumRepository extends BaseRepository {
  // Maps a relative URL to an absolute one.
  private mapUrl: (relative: string) => string;

  private urlMapper: UrlMapper;

  constructor(
    baseUrl: string,
    languagePreference = ContentLanguagePreference.Default,
  ) {
    super(baseUrl, languagePreference);

    this.urlMapper = new UrlMapper(baseUrl);

    this.mapUrl = (relative) => {
      return `${functions.mergeUrls(baseUrl, '/Album')}${relative}`;
    };
  }

  public createComment = (
    albumId: number,
    contract: CommentContract,
    callback: (contract: CommentContract) => void,
  ) => {
    $.postJSON(
      this.urlMapper.mapRelative(`/api/albums/${albumId}/comments`),
      contract,
      callback,
      'json',
    );
  };

  public createOrUpdateReview(
    albumId: number,
    reviewContract: AlbumReviewContract,
  ) {
    const url = functions.mergeUrls(
      this.baseUrl,
      `/api/albums/${albumId}/reviews`,
    );
    return this.handleJqueryPromise<AlbumReviewContract>(
      $.postJSON(url, reviewContract, null, 'json'),
    );
  }

  public createReport = (
    albumId: number,
    reportType: string,
    notes: string,
    versionNumber: number,
    callback?: () => void,
  ) => {
    $.post(
      this.urlMapper.mapRelative('/Album/CreateReport'),
      {
        reportType: reportType,
        notes: notes,
        albumId: albumId,
        versionNumber: versionNumber,
      },
      callback,
      'json',
    );
  };

  public deleteComment = (commentId: number, callback?: () => void) => {
    $.ajax(this.urlMapper.mapRelative(`/api/albums/comments/${commentId}`), {
      type: 'DELETE',
      success: callback,
    });
  };

  public deleteReview(albumId: number, reviewId: number) {
    const url = functions.mergeUrls(
      this.baseUrl,
      `/api/albums/${albumId}/reviews/${reviewId}`,
    );
    return this.handleJqueryPromise($.ajax(url, { type: 'DELETE' }));
  }

  public findDuplicate = (
    params,
    callback: (result: DuplicateEntryResultContract[]) => void,
  ) => {
    var url = functions.mergeUrls(this.baseUrl, '/Album/FindDuplicate');
    $.getJSON(url, params, callback);
  };

  public getComments = (
    albumId: number,
    callback: (contract: CommentContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative(`/api/albums/${albumId}/comments`),
      callback,
    );
  };

  public getForEdit = (
    id: number,
    callback: (result: AlbumForEditContract) => void,
  ) => {
    var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}/for-edit`);
    $.getJSON(url, callback);
  };

  public getOne = (id: number, callback: (result: AlbumContract) => void) => {
    var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}`);
    $.getJSON(
      url,
      { fields: 'AdditionalNames', lang: this.languagePreferenceStr },
      callback,
    );
  };

  public getOneWithComponents = (
    id: number,
    fields: string,
    languagePreference: string,
    callback: (result: AlbumForApiContract) => void,
  ) => {
    var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}`);
    $.getJSON(
      url,
      { fields: fields, lang: this.languagePreferenceStr },
      callback,
    );
  };

  getList = (
    paging: PagingProperties,
    lang: string,
    query: string,
    sort: string,
    discTypes: string,
    tags: number[],
    childTags: boolean,
    artistIds: number[],
    artistParticipationStatus: string,
    childVoicebanks: boolean,
    includeMembers: boolean,
    fields: string,
    status: string,
    deleted: boolean,
    advancedFilters: AdvancedSearchFilter[],
    callback: (result: PartialFindResultContract<AlbumContract>) => void,
  ) => {
    var url = functions.mergeUrls(this.baseUrl, '/api/albums');
    var data = {
      start: paging.start,
      getTotalCount: paging.getTotalCount,
      maxResults: paging.maxEntries,
      query: query,
      fields: fields,
      lang: lang,
      nameMatchMode: 'Auto',
      sort: sort,
      discTypes: discTypes,
      tagId: tags,
      childTags: childTags || undefined,
      artistId: artistIds,
      artistParticipationStatus: artistParticipationStatus,
      childVoicebanks: childVoicebanks,
      includeMembers: includeMembers || undefined,
      status: status,
      deleted: deleted,
      advancedFilters: advancedFilters,
    };

    $.getJSON(url, data, callback);
  };

  public async getReviews(albumId: number) {
    const url = functions.mergeUrls(
      this.baseUrl,
      `/api/albums/${albumId}/reviews`,
    );
    return await this.getJsonPromise<AlbumReviewContract[]>(url);
  }

  public getTagSuggestions = (
    albumId: number,
    callback: (contract: TagUsageForApiContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative(`/api/albums/${albumId}/tagSuggestions`),
      callback,
    );
  };

  public async getUserCollections(albumId: number) {
    const url = functions.mergeUrls(
      this.baseUrl,
      `/api/albums/${albumId}/user-collections`,
    );
    const jqueryPromise = $.getJSON(url);

    const promise = Promise.resolve(jqueryPromise);
    return promise as Promise<AlbumForUserForApiContract[]>;
  }

  public updateComment = (
    commentId: number,
    contract: CommentContract,
    callback?: () => void,
  ) => {
    $.postJSON(
      this.urlMapper.mapRelative(`/api/albums/comments/${commentId}`),
      contract,
      callback,
      'json',
    );
  };

  public updatePersonalDescription = (
    albumId: number,
    text: string,
    author: ArtistContract,
  ) => {
    $.postJSON(
      this.urlMapper.mapRelative(
        `/api/albums/${albumId}/personal-description/`,
      ),
      {
        personalDescriptionText: text,
        personalDescriptionAuthor: author || undefined,
      },
      null,
      'json',
    );
  };
}

export interface AlbumQueryParams extends CommonQueryParams {
  discTypes: string;
}
