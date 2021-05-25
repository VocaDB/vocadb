import AlbumContract from '@DataContracts/Album/AlbumContract';
import AlbumForApiContract from '@DataContracts/Album/AlbumForApiContract';
import AlbumForEditContract from '@DataContracts/Album/AlbumForEditContract';
import AlbumReviewContract from '@DataContracts/Album/AlbumReviewContract';
import ArtistContract from '@DataContracts/Artist/ArtistContract';
import CommentContract from '@DataContracts/CommentContract';
import DuplicateEntryResultContract from '@DataContracts/DuplicateEntryResultContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import AlbumForUserForApiContract from '@DataContracts/User/AlbumForUserForApiContract';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import functions from '@Shared/GlobalFunctions';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import AdvancedSearchFilter from '@ViewModels/Search/AdvancedSearchFilter';
import $ from 'jquery';

import BaseRepository from './BaseRepository';
import { CommonQueryParams } from './BaseRepository';
import ICommentRepository from './ICommentRepository';

// Repository for managing albums and related objects.
// Corresponds to the AlbumController class.
export default class AlbumRepository
  extends BaseRepository
  implements ICommentRepository {
  // Maps a relative URL to an absolute one.
  private mapUrl: (relative: string) => string;

  private readonly urlMapper: UrlMapper;

  constructor(
    private readonly httpClient: HttpClient,
    baseUrl: string,
    languagePreference = ContentLanguagePreference.Default,
  ) {
    super(baseUrl, languagePreference);

    this.urlMapper = new UrlMapper(baseUrl);

    this.mapUrl = (relative): string => {
      return `${functions.mergeUrls(baseUrl, '/Album')}${relative}`;
    };
  }

  public createComment = (
    albumId: number,
    contract: CommentContract,
  ): Promise<CommentContract> => {
    return this.httpClient.post<CommentContract>(
      this.urlMapper.mapRelative(`/api/albums/${albumId}/comments`),
      contract,
    );
  };

  public createOrUpdateReview(
    albumId: number,
    reviewContract: AlbumReviewContract,
  ): Promise<AlbumReviewContract> {
    const url = functions.mergeUrls(
      this.baseUrl,
      `/api/albums/${albumId}/reviews`,
    );
    return this.httpClient.post<AlbumReviewContract>(url, reviewContract);
  }

  public createReport = (
    albumId: number,
    reportType: string,
    notes: string,
    versionNumber: number,
    callback?: () => void,
  ): void => {
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

  public deleteComment = (commentId: number): Promise<void> => {
    return this.httpClient.delete<void>(
      this.urlMapper.mapRelative(`/api/albums/comments/${commentId}`),
    );
  };

  public deleteReview(albumId: number, reviewId: number): Promise<void> {
    const url = functions.mergeUrls(
      this.baseUrl,
      `/api/albums/${albumId}/reviews/${reviewId}`,
    );
    return this.httpClient.delete(url);
  }

  public findDuplicate = (params: {
    term1: string;
    term2: string;
    term3: string;
  }): Promise<DuplicateEntryResultContract[]> => {
    var url = functions.mergeUrls(this.baseUrl, '/Album/FindDuplicate');
    return this.httpClient.get<DuplicateEntryResultContract[]>(url, params);
  };

  public getComments = (albumId: number): Promise<CommentContract[]> => {
    return this.httpClient.get<CommentContract[]>(
      this.urlMapper.mapRelative(`/api/albums/${albumId}/comments`),
    );
  };

  public getForEdit = (id: number): Promise<AlbumForEditContract> => {
    var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}/for-edit`);
    return this.httpClient.get<AlbumForEditContract>(url);
  };

  public getOne = (id: number): Promise<AlbumContract> => {
    var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}`);
    return this.httpClient.get<AlbumContract>(url, {
      fields: 'AdditionalNames',
      lang: this.languagePreferenceStr,
    });
  };

  public getOneWithComponents = (
    id: number,
    fields: string,
    languagePreference: string,
  ): Promise<AlbumForApiContract> => {
    var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}`);
    return this.httpClient.get<AlbumForApiContract>(url, {
      fields: fields,
      lang: this.languagePreferenceStr,
    });
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
  ): Promise<PartialFindResultContract<AlbumContract>> => {
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

    return this.httpClient.get<PartialFindResultContract<AlbumContract>>(
      url,
      data,
    );
  };

  public getReviews = (albumId: number): Promise<AlbumReviewContract[]> => {
    const url = functions.mergeUrls(
      this.baseUrl,
      `/api/albums/${albumId}/reviews`,
    );
    return this.httpClient.get<AlbumReviewContract[]>(url);
  };

  public getTagSuggestions = (
    albumId: number,
  ): Promise<TagUsageForApiContract[]> => {
    return this.httpClient.get<TagUsageForApiContract[]>(
      this.urlMapper.mapRelative(`/api/albums/${albumId}/tagSuggestions`),
    );
  };

  public async getUserCollections(
    albumId: number,
  ): Promise<AlbumForUserForApiContract[]> {
    const url = functions.mergeUrls(
      this.baseUrl,
      `/api/albums/${albumId}/user-collections`,
    );
    return this.httpClient.get<AlbumForUserForApiContract[]>(url);
  }

  public updateComment = (
    commentId: number,
    contract: CommentContract,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      this.urlMapper.mapRelative(`/api/albums/comments/${commentId}`),
      contract,
    );
  };

  public updatePersonalDescription = (
    albumId: number,
    text: string,
    author: ArtistContract,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      this.urlMapper.mapRelative(
        `/api/albums/${albumId}/personal-description/`,
      ),
      {
        personalDescriptionText: text,
        personalDescriptionAuthor: author || undefined,
      },
    );
  };
}

export interface AlbumQueryParams extends CommonQueryParams {
  discTypes: string;
}
