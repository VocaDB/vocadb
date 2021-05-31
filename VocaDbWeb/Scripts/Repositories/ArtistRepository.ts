import ArtistApiContract from '@DataContracts/Artist/ArtistApiContract';
import ArtistContract from '@DataContracts/Artist/ArtistContract';
import ArtistForEditContract from '@DataContracts/Artist/ArtistForEditContract';
import CommentContract from '@DataContracts/CommentContract';
import DuplicateEntryResultContract from '@DataContracts/DuplicateEntryResultContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import AjaxHelper from '@Helpers/AjaxHelper';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import HttpClient, { HeaderNames, MediaTypes } from '@Shared/HttpClient';
import AdvancedSearchFilter from '@ViewModels/Search/AdvancedSearchFilter';

import BaseRepository from './BaseRepository';
import { CommonQueryParams } from './BaseRepository';
import ICommentRepository from './ICommentRepository';

// Repository for managing artists and related objects.
// Corresponds to the ArtistController class.
export default class ArtistRepository
  extends BaseRepository
  implements ICommentRepository {
  // Maps a relative URL to an absolute one.
  private mapUrl: (relative: string) => string;

  constructor(private readonly httpClient: HttpClient) {
    super();

    this.mapUrl = (relative: string): string => {
      return `/Artist${relative}`;
    };

    this.findDuplicate = (params): Promise<DuplicateEntryResultContract[]> => {
      return this.httpClient.post<DuplicateEntryResultContract[]>(
        this.mapUrl('/FindDuplicate'),
        AjaxHelper.stringify(params),
        {
          headers: {
            [HeaderNames.ContentType]: MediaTypes.APPLICATION_FORM_URLENCODED,
          },
        },
      );
    };
  }

  public createComment = (
    artistId: number,
    contract: CommentContract,
  ): Promise<CommentContract> => {
    return this.httpClient.post<CommentContract>(
      `/api/artists/${artistId}/comments`,
      contract,
    );
  };

  public createReport = (
    artistId: number,
    reportType: string,
    notes: string,
    versionNumber: number,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      '/Artist/CreateReport',
      AjaxHelper.stringify({
        reportType: reportType,
        notes: notes,
        artistId: artistId,
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
    return this.httpClient.delete<void>(`/api/artists/comments/${commentId}`);
  };

  public findDuplicate: (
    params: any,
  ) => Promise<DuplicateEntryResultContract[]>;

  public getComments = (artistId: number): Promise<CommentContract[]> => {
    return this.httpClient.get<CommentContract[]>(
      `/api/artists/${artistId}/comments`,
    );
  };

  public getForEdit = (id: number): Promise<ArtistForEditContract> => {
    return this.httpClient.get<ArtistForEditContract>(
      `/api/artists/${id}/for-edit`,
    );
  };

  public getOne = (
    id: number,
    lang: ContentLanguagePreference,
  ): Promise<ArtistContract> => {
    return this.httpClient.get<ArtistContract>(`/api/artists/${id}`, {
      fields: 'AdditionalNames',
      lang: ContentLanguagePreference[lang],
    });
  };

  public getOneWithComponents = (
    id: number,
    fields: string,
    lang: ContentLanguagePreference,
  ): Promise<ArtistApiContract> => {
    return this.httpClient.get<ArtistApiContract>(`/api/artists/${id}`, {
      fields: fields,
      lang: ContentLanguagePreference[lang],
    });
  };

  public getList = (
    paging: PagingProperties,
    lang: ContentLanguagePreference,
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
    var data = {
      start: paging.start,
      getTotalCount: paging.getTotalCount,
      maxResults: paging.maxEntries,
      query: query,
      fields: fields,
      lang: ContentLanguagePreference[lang],
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
      '/api/artists',
      data,
    );
  };

  public getTagSuggestions = (
    artistId: number,
  ): Promise<TagUsageForApiContract[]> => {
    return this.httpClient.get<TagUsageForApiContract[]>(
      `/api/artists/${artistId}/tagSuggestions`,
    );
  };

  public updateComment = (
    commentId: number,
    contract: CommentContract,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      `/api/artists/comments/${commentId}`,
      contract,
    );
  };
}

export interface ArtistQueryParams extends CommonQueryParams {
  artistTypes: string;
}
