import AdvancedSearchFilter from '../ViewModels/Search/AdvancedSearchFilter';
import ArtistContract from '../DataContracts/Artist/ArtistContract';
import BaseRepository from './BaseRepository';
import CommentContract from '../DataContracts/CommentContract';
import { CommonQueryParams } from './BaseRepository';
import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import CountPerDayContract from '../DataContracts/Aggregate/CountPerDayContract';
import functions from '../Shared/GlobalFunctions';
import ICommentRepository from './ICommentRepository';
import LyricsForSongContract from '../DataContracts/Song/LyricsForSongContract';
import NewSongCheckResultContract from '../DataContracts/NewSongCheckResultContract';
import PagingProperties from '../DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import PVService from '../Models/PVs/PVService';
import RatedSongForUserForApiContract from '../DataContracts/User/RatedSongForUserForApiContract';
import SongApiContract from '../DataContracts/Song/SongApiContract';
import SongContract from '../DataContracts/Song/SongContract';
import SongForEditContract from '../DataContracts/Song/SongForEditContract';
import SongListBaseContract from '../DataContracts/SongListBaseContract';
import SongType from '../Models/Songs/SongType';
import SongVoteRating from '../Models/SongVoteRating';
import SongWithPVPlayerAndVoteContract from '../DataContracts/Song/SongWithPVPlayerAndVoteContract';
import TagUsageForApiContract from '../DataContracts/Tag/TagUsageForApiContract';
import TimeUnit from '../Models/Aggregate/TimeUnit';
import UrlMapper from '../Shared/UrlMapper';

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
  ) => {
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
    callback: (contract: CommentContract) => void,
  ) => {
    $.postJSON(
      this.urlMapper.mapRelative('/api/songs/' + songId + '/comments'),
      contract,
      callback,
      'json',
    );
  };

  public createReport = (
    songId: number,
    reportType: string,
    notes: string,
    versionNumber: number,
    callback?: () => void,
  ) => {
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

  public deleteComment = (commentId: number, callback?: () => void) => {
    $.ajax(this.urlMapper.mapRelative('/api/songs/comments/' + commentId), {
      type: 'DELETE',
      success: callback,
    });
  };

  public findDuplicate = (
    params,
    callback: (result: NewSongCheckResultContract) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative('/api/songs/findDuplicate'),
      params,
      callback,
    );
  };

  private get: (relative: string, params: any, callback: any) => void;

  public getByNames(
    names: string[],
    ignoreIds: number[],
    songTypes?: SongType[],
  ) {
    const url = functions.mergeUrls(this.baseUrl, '/api/songs/by-names');
    const jqueryPromise = $.getJSON(url, {
      names: names,
      songTypes: songTypes,
      lang: this.languagePreferenceStr,
      ignoreIds: ignoreIds,
    });

    const promise = Promise.resolve(jqueryPromise);
    return promise as Promise<SongApiContract[]>;
  }

  public getComments = (
    songId: number,
    callback: (contract: CommentContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative('/api/songs/' + songId + '/comments'),
      callback,
    );
  };

  public getForEdit = (
    id: number,
    callback: (result: SongForEditContract) => void,
  ) => {
    var url = functions.mergeUrls(
      this.baseUrl,
      '/api/songs/' + id + '/for-edit',
    );
    $.getJSON(url, callback);
  };

  public getLyrics = (
    lyricsId: number,
    songVersion: number,
    callback: (contract: LyricsForSongContract) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative(
        '/api/songs/lyrics/' + lyricsId + '?v=' + songVersion,
      ),
      callback,
    );
  };

  private getJSON: (relative: string, params: any, callback: any) => void;

  public getOneWithComponents = (
    id: number,
    fields: string,
    languagePreference: string,
    callback?: (result: SongApiContract) => void,
  ) => {
    var url = functions.mergeUrls(this.baseUrl, '/api/songs/' + id);
    $.getJSON(
      url,
      {
        fields: fields,
        lang: languagePreference || this.languagePreferenceStr,
      },
      callback,
    );
  };

  public getOne = (id: number, callback?: (result: SongContract) => void) => {
    var url = functions.mergeUrls(this.baseUrl, '/api/songs/' + id);
    $.getJSON(
      url,
      { fields: 'AdditionalNames', lang: this.languagePreferenceStr },
      callback,
    );
  };

  public getListByParams(params: SongQueryParams, callback) {
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
    callback,
  ) => {
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

    $.getJSON(url, data, callback);
  };

  public getOverTime = (
    timeUnit: TimeUnit,
    artistId: number,
    callback?: (points: CountPerDayContract[]) => void,
  ) => {
    var url = this.urlMapper.mapRelative('/api/songs/over-time');
    return $.getJSON(
      url,
      { timeUnit: TimeUnit[timeUnit], artistId: artistId },
      callback,
    );
  };

  // Get PV ID by song ID and PV service.
  public getPvId = (
    songId: number,
    pvService: PVService,
    callback: (pvId: string) => void,
  ) => {
    return $.getJSON(
      this.urlMapper.mapRelative('/api/songs/' + songId + '/pvs'),
      { service: PVService[pvService] },
      callback,
    );
  };

  public getRatings = (
    songId: number,
    callback: (ratings: RatedSongForUserForApiContract[]) => void,
  ) => {
    return $.getJSON(
      this.urlMapper.mapRelative('/api/songs/' + songId + '/ratings'),
      { userFields: 'MainPicture' },
      callback,
    );
  };

  public getTagSuggestions = (
    songId: number,
    callback: (contract: TagUsageForApiContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative('/api/songs/' + songId + '/tagSuggestions'),
      callback,
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
    callback: (result: SongWithPVPlayerAndVoteContract) => void,
  ) => {
    this.getJSON('/PVPlayer/' + songId, params, callback);
  };

  public pvPlayerWithRating: (
    songId: number,
    callback: (result: SongWithPVPlayerAndVoteContract) => void,
  ) => void;

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
    callback?: () => void,
  ) => {
    $.postJSON(
      this.urlMapper.mapRelative('/api/songs/comments/' + commentId),
      contract,
      callback,
      'json',
    );
  };

  public updatePersonalDescription = (
    songId: number,
    text: string,
    author: ArtistContract,
  ) => {
    $.postJSON(
      this.urlMapper.mapRelative(
        '/api/songs/' + songId + '/personal-description/',
      ),
      {
        personalDescriptionText: text,
        personalDescriptionAuthor: author || undefined,
      },
      null,
      'json',
    );
  };

  public updateSongRating = (
    songId: number,
    rating: SongVoteRating,
    callback: () => void,
  ) => {
    var url = this.urlMapper.mapRelative('/api/songs/' + songId + '/ratings');
    $.postJSON(url, { rating: SongVoteRating[rating] }, callback);
  };

  private urlMapper: UrlMapper;

  constructor(
    baseUrl: string,
    languagePreference = ContentLanguagePreference.Default,
  ) {
    super(baseUrl, languagePreference);

    this.urlMapper = new UrlMapper(baseUrl);

    this.get = (relative, params, callback) => {
      $.get(this.mapUrl(relative), params, callback);
    };

    this.getJSON = (relative, params, callback) => {
      $.getJSON(this.mapUrl(relative), params, callback);
    };

    this.mapUrl = (relative: string) => {
      return functions.mergeUrls(baseUrl, '/Song') + relative;
    };

    this.post = (relative, params, callback) => {
      $.post(this.mapUrl(relative), params, callback);
    };

    this.pvForSongAndService = (
      songId: number,
      pvService: PVService,
      callback: (result: string) => void,
    ) => {
      this.get(
        '/PVForSongAndService',
        { songId: songId, service: PVService[pvService] },
        callback,
      );
    };

    this.pvPlayerWithRating = (songId, callback) => {
      this.getJSON('/PVPlayerWithRating', { songId: songId }, callback);
    };

    this.songListsForSong = (songId, callback) => {
      this.get('/SongListsForSong', { songId: songId }, callback);
    };

    this.songListsForUser = (ignoreSongId, callback) => {
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
