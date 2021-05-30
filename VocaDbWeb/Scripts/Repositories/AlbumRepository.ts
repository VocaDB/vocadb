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
import AjaxHelper from '@Helpers/AjaxHelper';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import HttpClient, { HeaderNames, MediaTypes } from '@Shared/HttpClient';
import AdvancedSearchFilter from '@ViewModels/Search/AdvancedSearchFilter';

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

  constructor(private readonly httpClient: HttpClient) {
    super();

    this.mapUrl = (relative): string => {
      return `/Album${relative}`;
    };
  }

  public createComment = (
    albumId: number,
    contract: CommentContract,
  ): Promise<CommentContract> => {
    return this.httpClient.post<CommentContract>(
      `/api/albums/${albumId}/comments`,
      contract,
    );
  };

  public createOrUpdateReview(
    albumId: number,
    reviewContract: AlbumReviewContract,
  ): Promise<AlbumReviewContract> {
    return this.httpClient.post<AlbumReviewContract>(
      `/api/albums/${albumId}/reviews`,
      reviewContract,
    );
  }

  public createReport = (
    albumId: number,
    reportType: string,
    notes: string,
    versionNumber: number,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      '/Album/CreateReport',
      AjaxHelper.stringify({
        reportType: reportType,
        notes: notes,
        albumId: albumId,
        versionNumber: versionNumber,
      }),
      {
        headers: {
          [HeaderNames.ContentType]: MediaTypes.APPLICATION_FORM_URLENCODED,
        },
      },
    );
  };

  public deleteComment = (commentId: number): Promise<void> => {
    return this.httpClient.delete<void>(`/api/albums/comments/${commentId}`);
  };

  public deleteReview(albumId: number, reviewId: number): Promise<void> {
    return this.httpClient.delete(`/api/albums/${albumId}/reviews/${reviewId}`);
  }

  public findDuplicate = (params: {
    term1: string;
    term2: string;
    term3: string;
  }): Promise<DuplicateEntryResultContract[]> => {
    return this.httpClient.get<DuplicateEntryResultContract[]>(
      '/Album/FindDuplicate',
      params,
    );
  };

  public getComments = (albumId: number): Promise<CommentContract[]> => {
    return this.httpClient.get<CommentContract[]>(
      `/api/albums/${albumId}/comments`,
    );
  };

  public getForEdit = (id: number): Promise<AlbumForEditContract> => {
    return this.httpClient.get<AlbumForEditContract>(
      `/api/albums/${id}/for-edit`,
    );
  };

  public getOne = (
    id: number,
    lang: ContentLanguagePreference,
  ): Promise<AlbumContract> => {
    return this.httpClient.get<AlbumContract>(`/api/albums/${id}`, {
      fields: 'AdditionalNames',
      lang: ContentLanguagePreference[lang],
    });
  };

  public getOneWithComponents = (
    id: number,
    fields: string,
    lang: ContentLanguagePreference,
  ): Promise<AlbumForApiContract> => {
    return this.httpClient.get<AlbumForApiContract>(`/api/albums/${id}`, {
      fields: fields,
      lang: ContentLanguagePreference[lang],
    });
  };

  getList = (
    paging: PagingProperties,
    lang: ContentLanguagePreference,
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
    var data = {
      start: paging.start,
      getTotalCount: paging.getTotalCount,
      maxResults: paging.maxEntries,
      query: query,
      fields: fields,
      lang: ContentLanguagePreference[lang],
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
      '/api/albums',
      data,
    );
  };

  public getReviews = (albumId: number): Promise<AlbumReviewContract[]> => {
    return this.httpClient.get<AlbumReviewContract[]>(
      `/api/albums/${albumId}/reviews`,
    );
  };

  public getTagSuggestions = (
    albumId: number,
  ): Promise<TagUsageForApiContract[]> => {
    return this.httpClient.get<TagUsageForApiContract[]>(
      `/api/albums/${albumId}/tagSuggestions`,
    );
  };

  public async getUserCollections(
    albumId: number,
  ): Promise<AlbumForUserForApiContract[]> {
    return this.httpClient.get<AlbumForUserForApiContract[]>(
      `/api/albums/${albumId}/user-collections`,
    );
  }

  public updateComment = (
    commentId: number,
    contract: CommentContract,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      `/api/albums/comments/${commentId}`,
      contract,
    );
  };

  public updatePersonalDescription = (
    albumId: number,
    text: string,
    author: ArtistContract,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      `/api/albums/${albumId}/personal-description/`,
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
