import AdvancedSearchFilter from '../ViewModels/Search/AdvancedSearchFilter';
import AjaxHelper from '../Helpers/AjaxHelper';
import CommentContract from '../DataContracts/CommentContract';
import EntryType from '../Models/EntryType';
import ICommentRepository from './ICommentRepository';
import PagingProperties from '../DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import RatedSongForUserForApiContract from '../DataContracts/User/RatedSongForUserForApiContract';
import ReleaseEventContract from '../DataContracts/ReleaseEvents/ReleaseEventContract';
import SongListContract from '../DataContracts/Song/SongListContract';
import SongVoteRating from '../Models/SongVoteRating';
import TagBaseContract from '../DataContracts/Tag/TagBaseContract';
import TagSelectionContract from '../DataContracts/Tag/TagSelectionContract';
import TagUsageForApiContract from '../DataContracts/Tag/TagUsageForApiContract';
import { Tuple2 } from '../Helpers/HighchartsHelper';
import UrlMapper from '../Shared/UrlMapper';
import UserApiContract from '../DataContracts/User/UserApiContract';
import UserEventRelationshipType from '../Models/Users/UserEventRelationshipType';
import UserMessageSummaryContract from '../DataContracts/User/UserMessageSummaryContract';
import AlbumForUserForApiContract from '../DataContracts/User/AlbumForUserForApiContract';
import ArtistForUserForApiContract from '../DataContracts/User/ArtistForUserForApiContract';
import HttpClient from '../Shared/HttpClient';

// Repository for managing users and related objects.
// Corresponds to the UserController class.
export default class UserRepository implements ICommentRepository {
  public addFollowedTag = (tagId: number): Promise<void> => {
    return this.httpClient.post<void>(
      this.urlMapper.mapRelative(`/api/users/current/followedTags/${tagId}`),
    );
  };

  public createArtistSubscription = (
    artistId: number,
    callback?: () => void,
  ): void => {
    $.post(this.mapUrl('/AddArtistForUser'), { artistId: artistId }, callback);
  };

  public createComment = (
    userId: number,
    contract: CommentContract,
  ): Promise<CommentContract> => {
    return this.httpClient.post<CommentContract>(
      this.urlMapper.mapRelative(`/api/users/${userId}/profileComments`),
      contract,
    );
  };

  public createMessage = (
    userId: number,
    contract: UserApiContract,
  ): Promise<UserMessageSummaryContract> => {
    return this.httpClient.post<UserMessageSummaryContract>(
      this.urlMapper.mapRelative(`/api/users/${userId}/messages`),
      contract,
    );
  };

  public deleteArtistSubscription = (
    artistId: number,
    callback?: () => void,
  ): void => {
    $.post(
      this.mapUrl('/RemoveArtistFromUser'),
      { artistId: artistId },
      callback,
    );
  };

  public deleteComment = (commentId: number): Promise<void> => {
    return this.httpClient.delete<void>(
      this.urlMapper.mapRelative(`/api/users/profileComments/${commentId}`),
    );
  };

  public deleteEventForUser = (eventId: number): Promise<void> => {
    var url = this.urlMapper.mapRelative(
      `/api/users/current/events/${eventId}`,
    );
    return this.httpClient.delete<void>(url);
  };

  public deleteFollowedTag = (tagId: number): Promise<void> => {
    return this.httpClient.delete<void>(
      this.urlMapper.mapRelative(`/api/users/current/followedTags/${tagId}`),
    );
  };

  public deleteMessage = (messageId: number): void => {
    var url = this.urlMapper.mapRelative('/User/DeleteMessage');
    $.post(url, { messageId: messageId });
  };

  public deleteMessages = (userId: number, messageIds: number[]): void => {
    var url = this.urlMapper.mapRelative(`/api/users/${userId}/messages`);
    AjaxHelper.deleteJSON_Url(url, 'messageId', messageIds);
  };

  public getAlbumCollectionList = (
    userId: number,
    paging: PagingProperties,
    lang: string,
    query: string,
    tag: number,
    albumType: string,
    artistId: number,
    purchaseStatuses: string,
    releaseEventId: number,
    advancedFilters: AdvancedSearchFilter[],
    sort: string,
  ): Promise<PartialFindResultContract<AlbumForUserForApiContract>> => {
    var url = this.urlMapper.mapRelative(`/api/users/${userId}/albums`);
    var data = {
      start: paging.start,
      getTotalCount: paging.getTotalCount,
      maxResults: paging.maxEntries,
      query: query,
      tagId: tag,
      albumTypes: albumType,
      artistId: artistId,
      purchaseStatuses: purchaseStatuses,
      releaseEventId: releaseEventId || undefined,
      fields: 'AdditionalNames,MainPicture',
      lang: lang,
      nameMatchMode: 'Auto',
      sort: sort,
      advancedFilters: advancedFilters,
    };

    return this.httpClient.get<
      PartialFindResultContract<AlbumForUserForApiContract>
    >(url, data);
  };

  public getComments = async (userId: number): Promise<CommentContract[]> => {
    var url = this.urlMapper.mapRelative(
      `/api/users/${userId}/profileComments`,
    );
    var data = {
      start: 0,
      getTotalCount: false,
      maxResults: 300,
      userId: userId,
    };

    const result = await this.httpClient.get<
      PartialFindResultContract<CommentContract>
    >(url, data);
    return result.items;
  };

  public getEvents = (
    userId: number,
    relationshipType: UserEventRelationshipType,
  ): Promise<ReleaseEventContract[]> => {
    var url = this.urlMapper.mapRelative(`/api/users/${userId}/events`);
    return this.httpClient.get<ReleaseEventContract[]>(url, {
      relationshipType: relationshipType,
    });
  };

  public getFollowedArtistsList = (
    userId: number,
    paging: PagingProperties,
    lang: string,
    tagIds: number[],
    artistType: string,
  ): Promise<PartialFindResultContract<ArtistForUserForApiContract>> => {
    var url = this.urlMapper.mapRelative(
      `/api/users/${userId}/followedArtists`,
    );
    var data = {
      start: paging.start,
      getTotalCount: paging.getTotalCount,
      maxResults: paging.maxEntries,
      tagId: tagIds,
      fields: 'AdditionalNames,MainPicture',
      lang: lang,
      nameMatchMode: 'Auto',
      artistType: artistType,
    };

    return this.httpClient.get<
      PartialFindResultContract<ArtistForUserForApiContract>
    >(url, data);
  };

  public getList = (
    paging: PagingProperties,
    query: string,
    sort: string,
    groups: string,
    includeDisabled: boolean,
    onlyVerified: boolean,
    knowsLanguage: string,
    nameMatchMode: string,
    fields: string,
  ): Promise<PartialFindResultContract<UserApiContract>> => {
    var url = this.urlMapper.mapRelative('/api/users');
    var data = {
      start: paging.start,
      getTotalCount: paging.getTotalCount,
      maxResults: paging.maxEntries,
      query: query,
      nameMatchMode: nameMatchMode,
      sort: sort,
      includeDisabled: includeDisabled,
      onlyVerified: onlyVerified,
      knowsLanguage: knowsLanguage,
      groups: groups || undefined,
      fields: fields || undefined,
    };

    return this.httpClient.get<PartialFindResultContract<UserApiContract>>(
      url,
      data,
    );
  };

  public getOne = (id: number, fields: string): Promise<UserApiContract> => {
    var url = this.urlMapper.mapRelative(`/api/users/${id}`);
    return this.httpClient.get<UserApiContract>(url, {
      fields: fields || undefined,
    });
  };

  public getOneByName = async (
    username: string,
  ): Promise<UserApiContract | null> => {
    const result = await this.getList(
      {},
      username,
      null!,
      null!,
      false,
      false,
      null!,
      'Exact',
      null!,
    );
    return result.items.length === 1 ? result.items[0] : null;
  };

  public getMessage = (
    messageId: number,
  ): Promise<UserMessageSummaryContract> => {
    var url = this.urlMapper.mapRelative(`/api/users/messages/${messageId}`);
    return this.httpClient.get<UserMessageSummaryContract>(url);
  };

  public getMessageSummaries = (
    userId: number,
    inbox: UserInboxType,
    paging: PagingProperties,
    unread: boolean = false,
    anotherUserId?: number,
    iconSize: number = 40,
  ): Promise<PartialFindResultContract<UserMessageSummaryContract>> => {
    var url = this.urlMapper.mapRelative(
      `/api/users/${userId || this.loggedUserId}/messages`,
    );
    return this.httpClient.get<
      PartialFindResultContract<UserMessageSummaryContract>
    >(url, {
      inbox: UserInboxType[inbox],
      start: paging.start,
      maxResults: paging.maxEntries,
      getTotalCount: paging.getTotalCount,
      unread: unread,
      anotherUserId: anotherUserId,
    });
  };

  public getRatedSongsList = (
    userId: number,
    paging: PagingProperties,
    lang: string,
    query: string,
    tagIds: number[],
    artistIds: number[],
    childVoicebanks: boolean,
    rating: string,
    songListId: number,
    advancedFilters: AdvancedSearchFilter[],
    groupByRating: boolean,
    pvServices: string,
    fields: string,
    sort: string,
  ): Promise<PartialFindResultContract<RatedSongForUserForApiContract>> => {
    var url = this.urlMapper.mapRelative(`/api/users/${userId}/ratedSongs`);
    var data = {
      start: paging.start,
      getTotalCount: paging.getTotalCount,
      maxResults: paging.maxEntries,
      query: query,
      tagId: tagIds,
      artistId: artistIds,
      childVoicebanks: childVoicebanks,
      rating: rating,
      songListId: songListId,
      advancedFilters: advancedFilters,
      groupByRating: groupByRating,
      pvServices: pvServices,
      fields: fields,
      lang: lang,
      nameMatchMode: 'Auto',
      sort: sort,
    };

    return this.httpClient.get<
      PartialFindResultContract<RatedSongForUserForApiContract>
    >(url, data);
  };

  public getRatingsByGenre = (
    userId: number,
  ): Promise<Tuple2<string, number>[]> => {
    var url = this.urlMapper.mapRelative(
      `/api/users/${userId}/songs-per-genre/`,
    );
    return this.httpClient.get<Tuple2<string, number>[]>(url);
  };

  public getSongLists = (
    userId: number,
    query: string,
    paging: PagingProperties,
    tagIds: number[],
    sort: string,
    fields: string,
  ): Promise<PartialFindResultContract<SongListContract>> => {
    var url = this.urlMapper.mapRelative(`/api/users/${userId}/songLists`);
    return this.httpClient.get<PartialFindResultContract<SongListContract>>(
      url,
      {
        query: query,
        start: paging.start,
        getTotalCount: paging.getTotalCount,
        maxResults: paging.maxEntries,
        tagId: tagIds,
        sort: sort,
        fields: fields,
      },
    );
  };

  // Gets a specific user's rating for a specific song.
  // userId: User ID. Can be null, in which case the logged user will be used (if any).
  // songId: ID of the song for which to get the rating. Cannot be null.
  // callback: Callback receiving the rating. If the user has not rated the song, or if the user is not logged in, this will be "Nothing".
  public getSongRating = (userId: number, songId: number): Promise<string> => {
    userId = userId || this.loggedUserId!;

    if (!userId) {
      return Promise.resolve('Nothing');
    }

    var url = this.urlMapper.mapRelative(
      `/api/users/${userId}/ratedSongs/${songId}`,
    );
    return this.httpClient.get<string>(url);
  };

  public getAlbumTagSelections = (
    albumId: number,
  ): Promise<TagSelectionContract[]> => {
    return this.httpClient.get<TagSelectionContract[]>(
      this.urlMapper.mapRelative(`/api/users/current/albumTags/${albumId}`),
    );
  };

  public getArtistTagSelections = (
    artistId: number,
  ): Promise<TagSelectionContract[]> => {
    return this.httpClient.get<TagSelectionContract[]>(
      this.urlMapper.mapRelative(`/api/users/current/artistTags/${artistId}`),
    );
  };

  public getEventTagSelections = (
    eventId: number,
  ): Promise<TagSelectionContract[]> => {
    return this.httpClient.get<TagSelectionContract[]>(
      this.urlMapper.mapRelative(`/api/users/current/eventTags/${eventId}`),
    );
  };

  public getEventSeriesTagSelections = (
    seriesId: number,
  ): Promise<TagSelectionContract[]> => {
    return this.httpClient.get<TagSelectionContract[]>(
      this.urlMapper.mapRelative(
        `/api/users/current/eventSeriesTags/${seriesId}`,
      ),
    );
  };

  public getSongListTagSelections = (
    songListId: number,
  ): Promise<TagSelectionContract[]> => {
    return this.httpClient.get<TagSelectionContract[]>(
      this.urlMapper.mapRelative(
        `/api/users/current/songListTags/${songListId}`,
      ),
    );
  };

  public getSongTagSelections = (
    songId: number,
  ): Promise<TagSelectionContract[]> => {
    return this.httpClient.get<TagSelectionContract[]>(
      this.urlMapper.mapRelative(`/api/users/current/songTags/${songId}`),
    );
  };

  public refreshEntryEdit = (
    entryType: EntryType,
    entryId: number,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      this.urlMapper.mapRelative(
        `/api/users/current/refreshEntryEdit/?entryType=${EntryType[entryType]}&entryId=${entryId}`,
      ),
    );
  };

  public requestEmailVerification = (callback?: () => void): void => {
    var url = this.mapUrl('/RequestEmailVerification');
    $.post(url, callback);
  };

  public updateAlbumTags = (
    albumId: number,
    tags: TagBaseContract[],
    callback: (usages: TagUsageForApiContract[]) => void,
  ): void => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative(`/api/users/current/albumTags/${albumId}`),
      tags,
      callback,
    );
  };

  // Updates artist subscription settings for an artist followed by a user.
  public updateArtistSubscription = (
    artistId: number,
    emailNotifications?: boolean,
    siteNotifications?: boolean,
  ): void => {
    $.post(this.mapUrl('/UpdateArtistSubscription'), {
      artistId: artistId,
      emailNotifications: emailNotifications,
      siteNotifications: siteNotifications,
    });
  };

  public updateArtistTags = (
    artistId: number,
    tags: TagBaseContract[],
    callback: (usages: TagUsageForApiContract[]) => void,
  ): void => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative(`/api/users/current/artistTags/${artistId}`),
      tags,
      callback,
    );
  };

  public updateComment = (
    commentId: number,
    contract: CommentContract,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      this.urlMapper.mapRelative(`/api/users/profileComments/${commentId}`),
      contract,
    );
  };

  public updateEventForUser = (
    eventId: number,
    associationType: UserEventRelationshipType,
  ): Promise<void> => {
    var url = this.urlMapper.mapRelative(
      `/api/users/current/events/${eventId}`,
    );
    return this.httpClient.post<void>(url, {
      associationType: UserEventRelationshipType[associationType],
    });
  };

  public updateEventTags = (
    eventId: number,
    tags: TagBaseContract[],
    callback: (usages: TagUsageForApiContract[]) => void,
  ): void => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative(`/api/users/current/eventTags/${eventId}`),
      tags,
      callback,
    );
  };

  public updateEventSeriesTags = (
    seriesId: number,
    tags: TagBaseContract[],
    callback: (usages: TagUsageForApiContract[]) => void,
  ): void => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative(
        `/api/users/current/eventSeriesTags/${seriesId}`,
      ),
      tags,
      callback,
    );
  };

  public updateSongListTags = (
    songListId: number,
    tags: TagBaseContract[],
    callback: (usages: TagUsageForApiContract[]) => void,
  ): void => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative(
        `/api/users/current/songListTags/${songListId}`,
      ),
      tags,
      callback,
    );
  };

  // Updates rating score for a song.
  // songId: Id of the song to be updated.
  // rating: Song rating.
  // callback: Callback function to be executed when the operation is complete.
  public updateSongRating = (
    songId: number,
    rating: SongVoteRating,
  ): Promise<void> => {
    var url = this.urlMapper.mapRelative(`/api/songs/${songId}/ratings`);
    return this.httpClient.post<void>(url, { rating: SongVoteRating[rating] });
  };

  public updateSongTags = (
    songId: number,
    tags: TagBaseContract[],
    callback: (usages: TagUsageForApiContract[]) => void,
  ): void => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative(`/api/users/current/songTags/${songId}`),
      tags,
      callback,
    );
  };

  // Updates user setting.
  // userId: user ID. Can be null in which case logged user ID (if any) will be used.
  // settingName: name of the setting to be updated, for example 'showChatBox'.
  // value: setting value, for example 'false'.
  public updateUserSetting = (
    userId: number,
    settingName: string,
    value: string,
    callback: () => void,
  ): void => {
    var url = this.urlMapper.mapRelative(
      `/api/users/${userId || this.loggedUserId}/settings/${settingName}`,
    );
    $.postJSON(url, value, callback);
  };

  // Maps a relative URL to an absolute one.
  private mapUrl: (relative: string) => string;

  constructor(
    private readonly httpClient: HttpClient,
    private urlMapper: UrlMapper,
    private loggedUserId?: number,
  ) {
    this.mapUrl = (relative: string): string => {
      return `${urlMapper.mapRelative('/User')}${relative}`;
    };
  }
}

export enum UserInboxType {
  Nothing,
  Received,
  Sent,
  Notifications,
}
