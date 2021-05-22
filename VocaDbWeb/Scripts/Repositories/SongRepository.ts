import CountPerDayContract from '@DataContracts/Aggregate/CountPerDayContract';
import ArtistContract from '@DataContracts/Artist/ArtistContract';
import CommentContract from '@DataContracts/CommentContract';
import NewSongCheckResultContract from '@DataContracts/NewSongCheckResultContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import LyricsForSongContract from '@DataContracts/Song/LyricsForSongContract';
import SongApiContract from '@DataContracts/Song/SongApiContract';
import SongContract from '@DataContracts/Song/SongContract';
import SongForEditContract from '@DataContracts/Song/SongForEditContract';
import SongWithPVPlayerAndVoteContract from '@DataContracts/Song/SongWithPVPlayerAndVoteContract';
import SongListBaseContract from '@DataContracts/SongListBaseContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import RatedSongForUserForApiContract from '@DataContracts/User/RatedSongForUserForApiContract';
import TimeUnit from '@Models/Aggregate/TimeUnit';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import PVService from '@Models/PVs/PVService';
import SongVoteRating from '@Models/SongVoteRating';
import SongType from '@Models/Songs/SongType';
import functions from '@Shared/GlobalFunctions';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import AdvancedSearchFilter from '@ViewModels/Search/AdvancedSearchFilter';
import $ from 'jquery';

import BaseRepository from './BaseRepository';
import { CommonQueryParams } from './BaseRepository';
import ICommentRepository from './ICommentRepository';

// Repository for managing songs and related objects.
// Corresponds to the SongController class.
export default class SongRepository
  extends BaseRepository
  implements ICommentRepository {
  public addSongToList = (
    listId: number,
    songId: number,
    notes: string,
    newListName: string,
    callback?: Function,
  ): void => {
    this.post(
      '/AddSongToList',
      {
        listId: listId,
        songId: songId,
        notes: notes,
        newListName: newListName,
      },
      callback,
    );
  };

  public createComment = (
    songId: number,
    contract: CommentContract,
  ): Promise<CommentContract> => {
    return this.httpClient.post<CommentContract>(
      this.urlMapper.mapRelative(`/api/songs/${songId}/comments`),
      contract,
    );
  };

  public createReport = (
    songId: number,
    reportType: string,
    notes: string,
    versionNumber: number,
    callback?: () => void,
  ): void => {
    $.post(
      this.urlMapper.mapRelative('/Song/CreateReport'),
      {
        reportType: reportType,
        notes: notes,
        songId: songId,
        versionNumber: versionNumber,
      },
      callback,
      'json',
    );
  };

  public deleteComment = (commentId: number): Promise<void> => {
    return this.httpClient.delete<void>(
      this.urlMapper.mapRelative(`/api/songs/comments/${commentId}`),
    );
  };

  public findDuplicate = (params: {
    term: string[];
    pv: string[];
    artistIds: number[];
    getPVInfo: boolean;
  }): Promise<NewSongCheckResultContract> => {
    return this.httpClient.get<NewSongCheckResultContract>(
      this.urlMapper.mapRelative('/api/songs/findDuplicate'),
      params,
    );
  };

  private get: (relative: string, params: any, callback: any) => void;

  public getByNames(
    names: string[],
    ignoreIds: number[],
    songTypes?: SongType[],
  ): Promise<SongApiContract[]> {
    const url = functions.mergeUrls(this.baseUrl, '/api/songs/by-names');
    return this.httpClient.get<SongApiContract[]>(url, {
      names: names,
      songTypes: songTypes,
      lang: this.languagePreferenceStr,
      ignoreIds: ignoreIds,
    });
  }

  public getComments = (songId: number): Promise<CommentContract[]> => {
    return this.httpClient.get<CommentContract[]>(
      this.urlMapper.mapRelative(`/api/songs/${songId}/comments`),
    );
  };

  public getForEdit = (id: number): Promise<SongForEditContract> => {
    var url = functions.mergeUrls(this.baseUrl, `/api/songs/${id}/for-edit`);
    return this.httpClient.get<SongForEditContract>(url);
  };

  public getLyrics = (
    lyricsId: number,
    songVersion: number,
  ): Promise<LyricsForSongContract> => {
    return this.httpClient.get<LyricsForSongContract>(
      this.urlMapper.mapRelative(
        `/api/songs/lyrics/${lyricsId}?v=${songVersion}`,
      ),
    );
  };

  private getJSON: <T>(relative: string, params: any) => Promise<T>;

  public getOneWithComponents = (
    id: number,
    fields: string,
    languagePreference: string,
  ): Promise<SongApiContract> => {
    var url = functions.mergeUrls(this.baseUrl, `/api/songs/${id}`);
    return this.httpClient.get<SongApiContract>(url, {
      fields: fields,
      lang: languagePreference || this.languagePreferenceStr,
    });
  };

  public getOne = (id: number): Promise<SongContract> => {
    var url = functions.mergeUrls(this.baseUrl, `/api/songs/${id}`);
    return this.httpClient.get<SongContract>(url, {
      fields: 'AdditionalNames',
      lang: this.languagePreferenceStr,
    });
  };

  public getListByParams(
    params: SongQueryParams,
    callback: any,
  ): Promise<PartialFindResultContract<SongApiContract>> {
    const url = functions.mergeUrls(this.baseUrl, '/api/songs');
    const jqueryPromise = $.getJSON(url, params);

    const promise = Promise.resolve(jqueryPromise);

    return promise as Promise<PartialFindResultContract<SongApiContract>>;

    //jqueryPromise.then(result => promise.)
  }

  public getList = (
    paging: PagingProperties,
    lang: string,
    query: string,
    sort: string,
    songTypes: string,
    afterDate: Date,
    beforeDate: Date,
    tagIds: number[],
    childTags: boolean,
    unifyTypesAndTags: boolean,
    artistIds: number[],
    artistParticipationStatus: string,
    childVoicebanks: boolean,
    includeMembers: boolean,
    eventId: number,
    onlyWithPvs: boolean,
    pvServices: string,
    since: number,
    minScore: number,
    userCollectionId: number,
    parentSongId: number,
    fields: string,
    status: string,
    advancedFilters: AdvancedSearchFilter[],
    minMilliBpm: number,
    maxMilliBpm: number,
    minLength: number,
    maxLength: number,
  ): Promise<PartialFindResultContract<SongContract>> => {
    var url = functions.mergeUrls(this.baseUrl, '/api/songs');
    var data = {
      start: paging.start,
      getTotalCount: paging.getTotalCount,
      maxResults: paging.maxEntries,
      query: query,
      fields: fields,
      lang: lang,
      nameMatchMode: 'Auto',
      sort: sort,
      songTypes: songTypes,
      afterDate: this.getDate(afterDate),
      beforeDate: this.getDate(beforeDate),
      tagId: tagIds,
      childTags: childTags,
      unifyTypesAndTags: unifyTypesAndTags || undefined,
      artistId: artistIds,
      artistParticipationStatus: artistParticipationStatus || undefined,
      childVoicebanks: childVoicebanks || undefined,
      includeMembers: includeMembers || undefined,
      releaseEventId: eventId,
      onlyWithPvs: onlyWithPvs,
      pvServices: pvServices,
      since: since,
      minScore: minScore,
      userCollectionId: userCollectionId,
      parentSongId: parentSongId || undefined,
      status: status,
      advancedFilters: advancedFilters,
      minMilliBpm: minMilliBpm,
      maxMilliBpm: maxMilliBpm,
      minLength: minLength,
      maxLength: maxLength,
    };

    return this.httpClient.get<PartialFindResultContract<SongContract>>(
      url,
      data,
    );
  };

  public getOverTime = (
    timeUnit: TimeUnit,
    artistId: number,
    callback?: (points: CountPerDayContract[]) => void,
  ): JQueryXHR => {
    var url = this.urlMapper.mapRelative('/api/songs/over-time');
    return $.getJSON(
      url,
      { timeUnit: TimeUnit[timeUnit], artistId: artistId },
      callback,
    );
  };

  // Get PV ID by song ID and PV service.
  public getPvId = (songId: number, pvService: PVService): Promise<string> => {
    return this.httpClient.get<string>(
      this.urlMapper.mapRelative(`/api/songs/${songId}/pvs`),
      { service: PVService[pvService] },
    );
  };

  public getRatings = (
    songId: number,
  ): Promise<RatedSongForUserForApiContract[]> => {
    return this.httpClient.get<RatedSongForUserForApiContract[]>(
      this.urlMapper.mapRelative(`/api/songs/${songId}/ratings`),
      { userFields: 'MainPicture' },
    );
  };

  public getTagSuggestions = (
    songId: number,
  ): Promise<TagUsageForApiContract[]> => {
    return this.httpClient.get<TagUsageForApiContract[]>(
      this.urlMapper.mapRelative(`/api/songs/${songId}/tagSuggestions`),
    );
  };

  // Maps a relative URL to an absolute one.
  private mapUrl: (relative: string) => string;

  private post: (relative: string, params: any, callback: any) => void;

  public pvForSongAndService: (
    songId: number,
    pvService: PVService,
    callback: (result: string) => void,
  ) => void;

  public pvPlayer = (
    songId: number,
    params: PVEmbedParams,
  ): Promise<SongWithPVPlayerAndVoteContract> => {
    return this.getJSON<SongWithPVPlayerAndVoteContract>(
      `/PVPlayer/${songId}`,
      params,
    );
  };

  public pvPlayerWithRating: (
    songId: number,
  ) => Promise<SongWithPVPlayerAndVoteContract>;

  //public songListsForSong: (songId: number, callback: (result: SongListContract[]) => void) => void;

  public songListsForSong: (
    songId: number,
    callback: (result: string) => void,
  ) => void;

  public songListsForUser: (
    ignoreSongId: number,
    callback: (result: SongListBaseContract[]) => void,
  ) => void;

  public updateComment = (
    commentId: number,
    contract: CommentContract,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      this.urlMapper.mapRelative(`/api/songs/comments/${commentId}`),
      contract,
    );
  };

  public updatePersonalDescription = (
    songId: number,
    text: string,
    author: ArtistContract,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      this.urlMapper.mapRelative(`/api/songs/${songId}/personal-description/`),
      {
        personalDescriptionText: text,
        personalDescriptionAuthor: author || undefined,
      },
    );
  };

  public updateSongRating = (
    songId: number,
    rating: SongVoteRating,
  ): Promise<void> => {
    var url = this.urlMapper.mapRelative(`/api/songs/${songId}/ratings`);
    return this.httpClient.post<void>(url, { rating: SongVoteRating[rating] });
  };

  private urlMapper: UrlMapper;

  constructor(
    private readonly httpClient: HttpClient,
    baseUrl: string,
    languagePreference = ContentLanguagePreference.Default,
  ) {
    super(baseUrl, languagePreference);

    this.urlMapper = new UrlMapper(baseUrl);

    this.get = (relative, params, callback): void => {
      $.get(this.mapUrl(relative), params, callback);
    };

    this.getJSON = <T>(relative: string, params: any): Promise<T> => {
      return this.httpClient.get<T>(this.mapUrl(relative), params);
    };

    this.mapUrl = (relative: string): string => {
      return `${functions.mergeUrls(baseUrl, '/Song')}${relative}`;
    };

    this.post = (relative, params, callback): void => {
      $.post(this.mapUrl(relative), params, callback);
    };

    this.pvForSongAndService = (
      songId: number,
      pvService: PVService,
      callback: (result: string) => void,
    ): void => {
      this.get(
        '/PVForSongAndService',
        { songId: songId, service: PVService[pvService] },
        callback,
      );
    };

    this.pvPlayerWithRating = (
      songId,
    ): Promise<SongWithPVPlayerAndVoteContract> => {
      return this.getJSON<SongWithPVPlayerAndVoteContract>(
        '/PVPlayerWithRating',
        { songId: songId },
      );
    };

    this.songListsForSong = (songId, callback): void => {
      this.get('/SongListsForSong', { songId: songId }, callback);
    };

    this.songListsForUser = (ignoreSongId, callback): void => {
      this.post('/SongListsForUser', { ignoreSongId: ignoreSongId }, callback);
    };
  }
}

export interface PVEmbedParams {
  enableScriptAccess?: boolean;

  elementId?: string;

  pvServices?: string;
}

export interface SongQueryParams extends CommonQueryParams {
  sort?: string;

  songTypes?: string;
}
