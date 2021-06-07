import CommentContract from '@DataContracts/CommentContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import SongListContract from '@DataContracts/Song/SongListContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import TagSelectionContract from '@DataContracts/Tag/TagSelectionContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import AlbumForUserForApiContract from '@DataContracts/User/AlbumForUserForApiContract';
import ArtistForUserForApiContract from '@DataContracts/User/ArtistForUserForApiContract';
import RatedSongForUserForApiContract from '@DataContracts/User/RatedSongForUserForApiContract';
import UserApiContract from '@DataContracts/User/UserApiContract';
import UserMessageSummaryContract from '@DataContracts/User/UserMessageSummaryContract';
import AjaxHelper from '@Helpers/AjaxHelper';
import { Tuple2 } from '@Helpers/HighchartsHelper';
import EntryType from '@Models/EntryType';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import SongVoteRating from '@Models/SongVoteRating';
import UserEventRelationshipType from '@Models/Users/UserEventRelationshipType';
import HttpClient, { HeaderNames, MediaTypes } from '@Shared/HttpClient';
import AdvancedSearchFilter from '@ViewModels/Search/AdvancedSearchFilter';

import ICommentRepository from './ICommentRepository';

export enum UserInboxType {
	Nothing,
	Received,
	Sent,
	Notifications,
}

// Repository for managing users and related objects.
// Corresponds to the UserController class.
export default class UserRepository implements ICommentRepository {
	// Maps a relative URL to an absolute one.
	private mapUrl: (relative: string) => string;

	constructor(private readonly httpClient: HttpClient) {
		this.mapUrl = (relative: string): string => {
			return `/User${relative}`;
		};
	}

	public addFollowedTag = (tagId: number): Promise<void> => {
		return this.httpClient.post<void>(
			`/api/users/current/followedTags/${tagId}`,
		);
	};

	public createArtistSubscription = (artistId: number): Promise<void> => {
		return this.httpClient.post<void>(
			this.mapUrl('/AddArtistForUser'),
			AjaxHelper.stringify({
				artistId: artistId,
			}),
			{
				headers: {
					[HeaderNames.ContentType]: MediaTypes.APPLICATION_FORM_URLENCODED,
				},
			},
		);
	};

	public createComment = (
		userId: number,
		contract: CommentContract,
	): Promise<CommentContract> => {
		return this.httpClient.post<CommentContract>(
			`/api/users/${userId}/profileComments`,
			contract,
		);
	};

	public createMessage = (
		userId: number,
		contract: UserApiContract,
	): Promise<UserMessageSummaryContract> => {
		return this.httpClient.post<UserMessageSummaryContract>(
			`/api/users/${userId}/messages`,
			contract,
		);
	};

	public deleteArtistSubscription = (artistId: number): Promise<void> => {
		return this.httpClient.post<void>(
			this.mapUrl('/RemoveArtistFromUser'),
			AjaxHelper.stringify({
				artistId: artistId,
			}),
			{
				headers: {
					[HeaderNames.ContentType]: MediaTypes.APPLICATION_FORM_URLENCODED,
				},
			},
		);
	};

	public deleteComment = (commentId: number): Promise<void> => {
		return this.httpClient.delete<void>(
			`/api/users/profileComments/${commentId}`,
		);
	};

	public deleteEventForUser = (eventId: number): Promise<void> => {
		return this.httpClient.delete<void>(`/api/users/current/events/${eventId}`);
	};

	public deleteFollowedTag = (tagId: number): Promise<void> => {
		return this.httpClient.delete<void>(
			`/api/users/current/followedTags/${tagId}`,
		);
	};

	public deleteMessage = (messageId: number): Promise<void> => {
		return this.httpClient.post<void>(
			'/User/DeleteMessage',
			AjaxHelper.stringify({ messageId: messageId }),
			{
				headers: {
					[HeaderNames.ContentType]: MediaTypes.APPLICATION_FORM_URLENCODED,
				},
			},
		);
	};

	public deleteMessages = (
		userId: number,
		messageIds: number[],
	): Promise<void> => {
		var dataParamName = 'messageId';
		var dataParam =
			'?' + dataParamName + '=' + messageIds.join('&' + dataParamName + '=');
		return this.httpClient.delete<void>(
			`/api/users/${userId}/messages${dataParam}`,
		);
	};

	public getAlbumCollectionList = (
		userId: number,
		paging: PagingProperties,
		lang: ContentLanguagePreference,
		query: string,
		tag: number,
		albumType: string,
		artistId: number,
		purchaseStatuses: string,
		releaseEventId: number,
		advancedFilters: AdvancedSearchFilter[],
		sort: string,
	): Promise<PartialFindResultContract<AlbumForUserForApiContract>> => {
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
			lang: ContentLanguagePreference[lang],
			nameMatchMode: 'Auto',
			sort: sort,
			advancedFilters: advancedFilters,
		};

		return this.httpClient.get<
			PartialFindResultContract<AlbumForUserForApiContract>
		>(`/api/users/${userId}/albums`, data);
	};

	public getComments = async (userId: number): Promise<CommentContract[]> => {
		var data = {
			start: 0,
			getTotalCount: false,
			maxResults: 300,
			userId: userId,
		};

		const result = await this.httpClient.get<
			PartialFindResultContract<CommentContract>
		>(`/api/users/${userId}/profileComments`, data);
		return result.items;
	};

	public getEvents = (
		userId: number,
		relationshipType: UserEventRelationshipType,
	): Promise<ReleaseEventContract[]> => {
		return this.httpClient.get<ReleaseEventContract[]>(
			`/api/users/${userId}/events`,
			{
				relationshipType: relationshipType,
			},
		);
	};

	public getFollowedArtistsList = (
		userId: number,
		paging: PagingProperties,
		lang: ContentLanguagePreference,
		tagIds: number[],
		artistType: string,
	): Promise<PartialFindResultContract<ArtistForUserForApiContract>> => {
		var data = {
			start: paging.start,
			getTotalCount: paging.getTotalCount,
			maxResults: paging.maxEntries,
			tagId: tagIds,
			fields: 'AdditionalNames,MainPicture',
			lang: ContentLanguagePreference[lang],
			nameMatchMode: 'Auto',
			artistType: artistType,
		};

		return this.httpClient.get<
			PartialFindResultContract<ArtistForUserForApiContract>
		>(`/api/users/${userId}/followedArtists`, data);
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
			'/api/users',
			data,
		);
	};

	public getOne = (id: number, fields: string): Promise<UserApiContract> => {
		return this.httpClient.get<UserApiContract>(`/api/users/${id}`, {
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
		return this.httpClient.get<UserMessageSummaryContract>(
			`/api/users/messages/${messageId}`,
		);
	};

	public getMessageSummaries = (
		userId: number,
		inbox: UserInboxType | undefined,
		paging: PagingProperties,
		unread: boolean = false,
		anotherUserId?: number,
		iconSize: number = 40,
	): Promise<PartialFindResultContract<UserMessageSummaryContract>> => {
		return this.httpClient.get<
			PartialFindResultContract<UserMessageSummaryContract>
		>(`/api/users/${userId}/messages`, {
			inbox: inbox ? UserInboxType[inbox] : undefined,
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
		lang: ContentLanguagePreference,
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
			lang: ContentLanguagePreference[lang],
			nameMatchMode: 'Auto',
			sort: sort,
		};

		return this.httpClient.get<
			PartialFindResultContract<RatedSongForUserForApiContract>
		>(`/api/users/${userId}/ratedSongs`, data);
	};

	public getRatingsByGenre = (
		userId: number,
	): Promise<Tuple2<string, number>[]> => {
		return this.httpClient.get<Tuple2<string, number>[]>(
			`/api/users/${userId}/songs-per-genre/`,
		);
	};

	public getSongLists = (
		userId: number,
		query: string,
		paging: PagingProperties,
		tagIds: number[],
		sort: string,
		fields: string,
	): Promise<PartialFindResultContract<SongListContract>> => {
		return this.httpClient.get<PartialFindResultContract<SongListContract>>(
			`/api/users/${userId}/songLists`,
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
	// userId: User ID.
	// songId: ID of the song for which to get the rating. Cannot be null.
	// callback: Callback receiving the rating. If the user has not rated the song, or if the user is not logged in, this will be "Nothing".
	public getSongRating = (userId: number, songId: number): Promise<string> => {
		if (!userId) {
			return Promise.resolve('Nothing');
		}

		return this.httpClient.get<string>(
			`/api/users/${userId}/ratedSongs/${songId}`,
		);
	};

	public getAlbumTagSelections = (
		albumId: number,
	): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			`/api/users/current/albumTags/${albumId}`,
		);
	};

	public getArtistTagSelections = (
		artistId: number,
	): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			`/api/users/current/artistTags/${artistId}`,
		);
	};

	public getEventTagSelections = (
		eventId: number,
	): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			`/api/users/current/eventTags/${eventId}`,
		);
	};

	public getEventSeriesTagSelections = (
		seriesId: number,
	): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			`/api/users/current/eventSeriesTags/${seriesId}`,
		);
	};

	public getSongListTagSelections = (
		songListId: number,
	): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			`/api/users/current/songListTags/${songListId}`,
		);
	};

	public getSongTagSelections = (
		songId: number,
	): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			`/api/users/current/songTags/${songId}`,
		);
	};

	public refreshEntryEdit = (
		entryType: EntryType,
		entryId: number,
	): Promise<void> => {
		return this.httpClient.post<void>(
			`/api/users/current/refreshEntryEdit/?entryType=${EntryType[entryType]}&entryId=${entryId}`,
		);
	};

	public requestEmailVerification = (): Promise<void> => {
		return this.httpClient.post<void>(
			this.mapUrl('/RequestEmailVerification'),
			undefined,
			{
				headers: {
					[HeaderNames.ContentType]: MediaTypes.APPLICATION_FORM_URLENCODED,
				},
			},
		);
	};

	public updateAlbumTags = (
		albumId: number,
		tags: TagBaseContract[],
	): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			`/api/users/current/albumTags/${albumId}`,
			tags,
		);
	};

	// Updates artist subscription settings for an artist followed by a user.
	public updateArtistSubscription = (
		artistId: number,
		emailNotifications?: boolean,
		siteNotifications?: boolean,
	): Promise<void> => {
		return this.httpClient.post<void>(
			this.mapUrl('/UpdateArtistSubscription'),
			AjaxHelper.stringify({
				artistId: artistId,
				emailNotifications: emailNotifications,
				siteNotifications: siteNotifications,
			}),
			{
				headers: {
					[HeaderNames.ContentType]: MediaTypes.APPLICATION_FORM_URLENCODED,
				},
			},
		);
	};

	public updateArtistTags = (
		artistId: number,
		tags: TagBaseContract[],
	): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			`/api/users/current/artistTags/${artistId}`,
			tags,
		);
	};

	public updateComment = (
		commentId: number,
		contract: CommentContract,
	): Promise<void> => {
		return this.httpClient.post<void>(
			`/api/users/profileComments/${commentId}`,
			contract,
		);
	};

	public updateEventForUser = (
		eventId: number,
		associationType: UserEventRelationshipType,
	): Promise<void> => {
		return this.httpClient.post<void>(`/api/users/current/events/${eventId}`, {
			associationType: UserEventRelationshipType[associationType],
		});
	};

	public updateEventTags = (
		eventId: number,
		tags: TagBaseContract[],
	): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			`/api/users/current/eventTags/${eventId}`,
			tags,
		);
	};

	public updateEventSeriesTags = (
		seriesId: number,
		tags: TagBaseContract[],
	): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			`/api/users/current/eventSeriesTags/${seriesId}`,
			tags,
		);
	};

	public updateSongListTags = (
		songListId: number,
		tags: TagBaseContract[],
	): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			`/api/users/current/songListTags/${songListId}`,
			tags,
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
		return this.httpClient.post<void>(`/api/songs/${songId}/ratings`, {
			rating: SongVoteRating[rating],
		});
	};

	public updateSongTags = (
		songId: number,
		tags: TagBaseContract[],
	): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			`/api/users/current/songTags/${songId}`,
			tags,
		);
	};

	// Updates user setting.
	// userId: user ID.
	// settingName: name of the setting to be updated, for example 'showChatBox'.
	// value: setting value, for example 'false'.
	public updateUserSetting = (
		userId: number,
		settingName: string,
		value: string,
	): Promise<void> => {
		return this.httpClient.post<void>(
			`/api/users/${userId}/settings/${settingName}`,
			`"${value}"`,
			{
				headers: { [HeaderNames.ContentType]: MediaTypes.APPLICATION_JSON },
			},
		);
	};
}
