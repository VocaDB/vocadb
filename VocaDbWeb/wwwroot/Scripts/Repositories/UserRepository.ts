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

// Repository for managing users and related objects.
// Corresponds to the UserController class.
export default class UserRepository implements ICommentRepository {
  public addFollowedTag = (tagId: number, callback?: () => void) => {
    $.postJSON(
      this.urlMapper.mapRelative('/api/users/current/followedTags/' + tagId),
      callback,
    );
  };

  public createArtistSubscription = (
    artistId: number,
    callback?: () => void,
  ) => {
    $.post(this.mapUrl('/AddArtistForUser'), { artistId: artistId }, callback);
  };

  public createComment = (
    userId: number,
    contract: CommentContract,
    callback: (contract: CommentContract) => void,
  ) => {
    $.postJSON(
      this.urlMapper.mapRelative('/api/users/' + userId + '/profileComments'),
      contract,
      callback,
      'json',
    );
  };

  public createMessage = (
    userId: number,
    contract: UserApiContract,
    callback: (result: UserMessageSummaryContract) => void,
  ) => {
    return $.postJSON(
      this.urlMapper.mapRelative('/api/users/' + userId + '/messages'),
      contract,
      callback,
      'json',
    );
  };

  public deleteArtistSubscription = (
    artistId: number,
    callback?: () => void,
  ) => {
    $.post(
      this.mapUrl('/RemoveArtistFromUser'),
      { artistId: artistId },
      callback,
    );
  };

  public deleteComment = (commentId: number, callback: () => void) => {
    $.ajax(
      this.urlMapper.mapRelative('/api/users/profileComments/' + commentId),
      { type: 'DELETE', success: callback },
    );
  };

  public deleteEventForUser = (eventId: number, callback?: () => void) => {
    var url = this.urlMapper.mapRelative(
      '/api/users/current/events/' + eventId,
    );
    return $.ajax(url, {
      type: 'DELETE',
      success: callback,
    }) as JQueryPromise<{}>;
  };

  public deleteFollowedTag = (tagId: number, callback?: () => void) => {
    $.ajax(
      this.urlMapper.mapRelative('/api/users/current/followedTags/' + tagId),
      { type: 'DELETE', success: callback },
    );
  };

  public deleteMessage = (messageId: number) => {
    var url = this.urlMapper.mapRelative('/User/DeleteMessage');
    $.post(url, { messageId: messageId });
  };

  public deleteMessages = (userId: number, messageIds: number[]) => {
    var url = this.urlMapper.mapRelative('/api/users/' + userId + '/messages');
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
    callback,
  ) => {
    var url = this.urlMapper.mapRelative('/api/users/' + userId + '/albums');
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

    $.getJSON(url, data, callback);
  };

  public getComments = (userId: number, callback) => {
    var url = this.urlMapper.mapRelative(
      '/api/users/' + userId + '/profileComments',
    );
    var data = {
      start: 0,
      getTotalCount: false,
      maxResults: 300,
      userId: userId,
    };

    $.getJSON(url, data, (result) => callback(result.items));
  };

  public getEvents = (
    userId: number,
    relationshipType: UserEventRelationshipType,
    callback: (result: ReleaseEventContract[]) => void,
  ) => {
    var url = this.urlMapper.mapRelative('/api/users/' + userId + '/events');
    $.getJSON(url, { relationshipType: relationshipType }, callback);
  };

  public getFollowedArtistsList = (
    userId: number,
    paging: PagingProperties,
    lang: string,
    tagIds: number[],
    artistType: string,
    callback,
  ) => {
    var url = this.urlMapper.mapRelative(
      '/api/users/' + userId + '/followedArtists',
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

    $.getJSON(url, data, callback);
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
    callback: (result: PartialFindResultContract<UserApiContract>) => void,
  ) => {
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

    $.getJSON(url, data, callback);
  };

  public getOne = (
    id: number,
    fields: string,
    callback: (result: UserApiContract) => void,
  ) => {
    var url = this.urlMapper.mapRelative('/api/users/' + id);
    $.getJSON(url, { fields: fields || undefined }, callback);
  };

  public getOneByName = (
    username: string,
    callback: (result: UserApiContract) => void,
  ) => {
    this.getList(
      {},
      username,
      null,
      null,
      false,
      false,
      null,
      'Exact',
      null,
      (result) => {
        callback(result.items.length === 1 ? result.items[0] : null);
      },
    );
  };

  public getMessage = (
    messageId: number,
    callback?: (result: UserMessageSummaryContract) => void,
  ) => {
    var url = this.urlMapper.mapRelative('/api/users/messages/' + messageId);
    $.getJSON(url, callback);
  };

  public getMessageSummaries = (
    userId: number,
    inbox: UserInboxType,
    paging: PagingProperties,
    unread: boolean = false,
    anotherUserId?: number,
    iconSize: number = 40,
    callback?: (
      result: PartialFindResultContract<UserMessageSummaryContract>,
    ) => void,
  ) => {
    var url = this.urlMapper.mapRelative(
      '/api/users/' + (userId || this.loggedUserId) + '/messages',
    );
    $.getJSON(
      url,
      {
        inbox: UserInboxType[inbox],
        start: paging.start,
        maxResults: paging.maxEntries,
        getTotalCount: paging.getTotalCount,
        unread: unread,
        anotherUserId: anotherUserId,
      },
      callback,
    );
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
    callback: (
      result: PartialFindResultContract<RatedSongForUserForApiContract>,
    ) => void,
  ) => {
    var url = this.urlMapper.mapRelative(
      '/api/users/' + userId + '/ratedSongs',
    );
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

    $.getJSON(url, data, callback);
  };

  public getRatingsByGenre = (
    userId: number,
    callback: (points: Tuple2<string, number>[]) => void,
  ) => {
    var url = this.urlMapper.mapRelative(
      '/api/users/' + userId + '/songs-per-genre/',
    );
    $.getJSON(url, (data) => {
      callback(data);
    });
  };

  public getSongLists = (
    userId: number,
    query: string,
    paging: PagingProperties,
    tagIds: number[],
    sort: string,
    fields: string,
    callback: (result: PartialFindResultContract<SongListContract>) => void,
  ) => {
    var url = this.urlMapper.mapRelative('/api/users/' + userId + '/songLists');
    $.getJSON(
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
      callback,
    );
  };

  // Gets a specific user's rating for a specific song.
  // userId: User ID. Can be null, in which case the logged user will be used (if any).
  // songId: ID of the song for which to get the rating. Cannot be null.
  // callback: Callback receiving the rating. If the user has not rated the song, or if the user is not logged in, this will be "Nothing".
  public getSongRating = (
    userId: number,
    songId: number,
    callback: (rating: string) => void,
  ) => {
    userId = userId || this.loggedUserId;

    if (!userId) {
      callback('Nothing');
      return;
    }

    var url = this.urlMapper.mapRelative(
      '/api/users/' + userId + '/ratedSongs/' + songId,
    );
    $.getJSON(url, callback);
  };

  public getAlbumTagSelections = (
    albumId: number,
    callback: (tags: TagSelectionContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative('/api/users/current/albumTags/' + albumId),
      callback,
    );
  };

  public getArtistTagSelections = (
    artistId: number,
    callback: (tags: TagSelectionContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative('/api/users/current/artistTags/' + artistId),
      callback,
    );
  };

  public getEventTagSelections = (
    eventId: number,
    callback: (tags: TagSelectionContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative('/api/users/current/eventTags/' + eventId),
      callback,
    );
  };

  public getEventSeriesTagSelections = (
    seriesId: number,
    callback: (tags: TagSelectionContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative(
        '/api/users/current/eventSeriesTags/' + seriesId,
      ),
      callback,
    );
  };

  public getSongListTagSelections = (
    songListId: number,
    callback: (tags: TagSelectionContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative(
        '/api/users/current/songListTags/' + songListId,
      ),
      callback,
    );
  };

  public getSongTagSelections = (
    songId: number,
    callback: (tags: TagSelectionContract[]) => void,
  ) => {
    $.getJSON(
      this.urlMapper.mapRelative('/api/users/current/songTags/' + songId),
      callback,
    );
  };

  public refreshEntryEdit = (entryType: EntryType, entryId: number) => {
    $.postJSON(
      this.urlMapper.mapRelative(
        '/api/users/current/refreshEntryEdit/?entryType=' +
          EntryType[entryType] +
          '&entryId=' +
          entryId,
      ),
    );
  };

  public requestEmailVerification = (callback?: () => void) => {
    var url = this.mapUrl('/RequestEmailVerification');
    $.post(url, callback);
  };

  public updateAlbumTags = (
    albumId: number,
    tags: TagBaseContract[],
    callback: (usages: TagUsageForApiContract[]) => void,
  ) => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative('/api/users/current/albumTags/' + albumId),
      tags,
      callback,
    );
  };

  // Updates artist subscription settings for an artist followed by a user.
  public updateArtistSubscription = (
    artistId: number,
    emailNotifications?: boolean,
    siteNotifications?: boolean,
  ) => {
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
  ) => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative('/api/users/current/artistTags/' + artistId),
      tags,
      callback,
    );
  };

  public updateComment = (
    commentId: number,
    contract: CommentContract,
    callback?: () => void,
  ) => {
    $.postJSON(
      this.urlMapper.mapRelative('/api/users/profileComments/' + commentId),
      contract,
      callback,
      'json',
    );
  };

  public updateEventForUser = (
    eventId: number,
    associationType: UserEventRelationshipType,
    callback?: () => void,
  ) => {
    var url = this.urlMapper.mapRelative(
      '/api/users/current/events/' + eventId,
    );
    return $.postJSON(
      url,
      { associationType: UserEventRelationshipType[associationType] },
      callback,
    ) as JQueryPromise<{}>;
  };

  public updateEventTags = (
    eventId: number,
    tags: TagBaseContract[],
    callback: (usages: TagUsageForApiContract[]) => void,
  ) => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative('/api/users/current/eventTags/' + eventId),
      tags,
      callback,
    );
  };

  public updateEventSeriesTags = (
    seriesId: number,
    tags: TagBaseContract[],
    callback: (usages: TagUsageForApiContract[]) => void,
  ) => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative(
        '/api/users/current/eventSeriesTags/' + seriesId,
      ),
      tags,
      callback,
    );
  };

  public updateSongListTags = (
    songListId: number,
    tags: TagBaseContract[],
    callback: (usages: TagUsageForApiContract[]) => void,
  ) => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative(
        '/api/users/current/songListTags/' + songListId,
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
    callback: () => void,
  ) => {
    var url = this.urlMapper.mapRelative('/api/songs/' + songId + '/ratings');
    return $.postJSON(
      url,
      { rating: SongVoteRating[rating] },
      callback,
    ) as JQueryPromise<any>;
  };

  public updateSongTags = (
    songId: number,
    tags: TagBaseContract[],
    callback: (usages: TagUsageForApiContract[]) => void,
  ) => {
    AjaxHelper.putJSON(
      this.urlMapper.mapRelative('/api/users/current/songTags/' + songId),
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
  ) => {
    var url = this.urlMapper.mapRelative(
      '/api/users/' +
        (userId || this.loggedUserId) +
        '/settings/' +
        settingName,
    );
    $.postJSON(url, value, callback);
  };

  // Maps a relative URL to an absolute one.
  private mapUrl: (relative: string) => string;

  constructor(private urlMapper: UrlMapper, private loggedUserId?: number) {
    this.mapUrl = (relative: string) => {
      return urlMapper.mapRelative('/User') + relative;
    };
  }
}

export enum UserInboxType {
  Nothing,
  Received,
  Sent,
  Notifications,
}
