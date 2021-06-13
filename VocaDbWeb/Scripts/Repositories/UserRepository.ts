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
import UrlMapper from '@Shared/UrlMapper';
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

	public constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {
		this.mapUrl = (relative: string): string => {
			return `${urlMapper.mapRelative('/User')}${relative}`;
		};
	}

	public addFollowedTag = ({ tagId }: { tagId: number }): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(`/api/users/current/followedTags/${tagId}`),
		);
	};

	public createArtistSubscription = ({
		artistId,
	}: {
		artistId: number;
	}): Promise<void> => {
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

	public createComment = ({
		entryId: userId,
		contract,
	}: {
		entryId: number;
		contract: CommentContract;
	}): Promise<CommentContract> => {
		return this.httpClient.post<CommentContract>(
			this.urlMapper.mapRelative(`/api/users/${userId}/profileComments`),
			contract,
		);
	};

	public createMessage = ({
		userId,
		contract,
	}: {
		userId: number;
		contract: UserApiContract;
	}): Promise<UserMessageSummaryContract> => {
		return this.httpClient.post<UserMessageSummaryContract>(
			this.urlMapper.mapRelative(`/api/users/${userId}/messages`),
			contract,
		);
	};

	public deleteArtistSubscription = ({
		artistId,
	}: {
		artistId: number;
	}): Promise<void> => {
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

	public deleteComment = ({
		commentId,
	}: {
		commentId: number;
	}): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(`/api/users/profileComments/${commentId}`),
		);
	};

	public deleteEventForUser = ({
		eventId,
	}: {
		eventId: number;
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative(
			`/api/users/current/events/${eventId}`,
		);
		return this.httpClient.delete<void>(url);
	};

	public deleteFollowedTag = ({ tagId }: { tagId: number }): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(`/api/users/current/followedTags/${tagId}`),
		);
	};

	public deleteMessage = ({
		messageId,
	}: {
		messageId: number;
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative('/User/DeleteMessage');
		return this.httpClient.post<void>(
			url,
			AjaxHelper.stringify({ messageId: messageId }),
			{
				headers: {
					[HeaderNames.ContentType]: MediaTypes.APPLICATION_FORM_URLENCODED,
				},
			},
		);
	};

	public deleteMessages = ({
		userId,
		messageIds,
	}: {
		userId: number;
		messageIds: number[];
	}): Promise<void> => {
		var dataParamName = 'messageId';
		var dataParam =
			'?' + dataParamName + '=' + messageIds.join('&' + dataParamName + '=');
		var url = this.urlMapper.mapRelative(
			`/api/users/${userId}/messages${dataParam}`,
		);
		return this.httpClient.delete<void>(url);
	};

	public getAlbumCollectionList = ({
		userId,
		paging,
		lang,
		query,
		tag,
		albumType,
		artistId,
		purchaseStatuses,
		releaseEventId,
		advancedFilters,
		sort,
	}: {
		userId: number;
		paging: PagingProperties;
		lang: ContentLanguagePreference;
		query: string;
		tag: number;
		albumType: string;
		artistId: number;
		purchaseStatuses: string;
		releaseEventId: number;
		advancedFilters: AdvancedSearchFilter[];
		sort: string;
	}): Promise<PartialFindResultContract<AlbumForUserForApiContract>> => {
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
			lang: ContentLanguagePreference[lang],
			nameMatchMode: 'Auto',
			sort: sort,
			advancedFilters: advancedFilters,
		};

		return this.httpClient.get<
			PartialFindResultContract<AlbumForUserForApiContract>
		>(url, data);
	};

	public getComments = async ({
		entryId: userId,
	}: {
		entryId: number;
	}): Promise<CommentContract[]> => {
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

	public getEvents = ({
		userId,
		relationshipType,
	}: {
		userId: number;
		relationshipType: UserEventRelationshipType;
	}): Promise<ReleaseEventContract[]> => {
		var url = this.urlMapper.mapRelative(`/api/users/${userId}/events`);
		return this.httpClient.get<ReleaseEventContract[]>(url, {
			relationshipType: relationshipType,
		});
	};

	public getFollowedArtistsList = ({
		userId,
		paging,
		lang,
		tagIds,
		artistType,
	}: {
		userId: number;
		paging: PagingProperties;
		lang: ContentLanguagePreference;
		tagIds: number[];
		artistType: string;
	}): Promise<PartialFindResultContract<ArtistForUserForApiContract>> => {
		var url = this.urlMapper.mapRelative(
			`/api/users/${userId}/followedArtists`,
		);
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
		>(url, data);
	};

	public getList = ({
		paging,
		query,
		sort,
		groups,
		includeDisabled,
		onlyVerified,
		knowsLanguage,
		nameMatchMode,
		fields,
	}: {
		paging: PagingProperties;
		query: string;
		sort?: string;
		groups?: string;
		includeDisabled: boolean;
		onlyVerified: boolean;
		knowsLanguage?: string;
		nameMatchMode: string;
		fields?: string;
	}): Promise<PartialFindResultContract<UserApiContract>> => {
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

	public getOne = ({
		id,
		fields,
	}: {
		id: number;
		fields?: string;
	}): Promise<UserApiContract> => {
		var url = this.urlMapper.mapRelative(`/api/users/${id}`);
		return this.httpClient.get<UserApiContract>(url, {
			fields: fields || undefined,
		});
	};

	public getOneByName = async ({
		username,
	}: {
		username: string;
	}): Promise<UserApiContract | null> => {
		const result = await this.getList({
			paging: {},
			query: username,
			sort: undefined,
			groups: undefined,
			includeDisabled: false,
			onlyVerified: false,
			knowsLanguage: undefined,
			nameMatchMode: 'Exact',
			fields: undefined,
		});
		return result.items.length === 1 ? result.items[0] : null;
	};

	public getMessage = ({
		messageId,
	}: {
		messageId: number;
	}): Promise<UserMessageSummaryContract> => {
		var url = this.urlMapper.mapRelative(`/api/users/messages/${messageId}`);
		return this.httpClient.get<UserMessageSummaryContract>(url);
	};

	public getMessageSummaries = ({
		userId,
		inbox,
		paging,
		unread = false,
		anotherUserId,
		iconSize = 40,
	}: {
		userId: number;
		inbox?: UserInboxType;
		paging: PagingProperties;
		unread: boolean;
		anotherUserId?: number;
		iconSize?: number;
	}): Promise<PartialFindResultContract<UserMessageSummaryContract>> => {
		var url = this.urlMapper.mapRelative(`/api/users/${userId}/messages`);
		return this.httpClient.get<
			PartialFindResultContract<UserMessageSummaryContract>
		>(url, {
			inbox: inbox ? UserInboxType[inbox] : undefined,
			start: paging.start,
			maxResults: paging.maxEntries,
			getTotalCount: paging.getTotalCount,
			unread: unread,
			anotherUserId: anotherUserId,
		});
	};

	public getRatedSongsList = ({
		userId,
		paging,
		lang,
		query,
		tagIds,
		artistIds,
		childVoicebanks,
		rating,
		songListId,
		advancedFilters,
		groupByRating,
		pvServices,
		fields,
		sort,
	}: {
		userId: number;
		paging: PagingProperties;
		lang: ContentLanguagePreference;
		query: string;
		tagIds: number[];
		artistIds: number[];
		childVoicebanks: boolean;
		rating: string;
		songListId: number;
		advancedFilters: AdvancedSearchFilter[];
		groupByRating: boolean;
		pvServices?: string;
		fields: string;
		sort: string;
	}): Promise<PartialFindResultContract<RatedSongForUserForApiContract>> => {
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
			lang: ContentLanguagePreference[lang],
			nameMatchMode: 'Auto',
			sort: sort,
		};

		return this.httpClient.get<
			PartialFindResultContract<RatedSongForUserForApiContract>
		>(url, data);
	};

	public getRatingsByGenre = ({
		userId,
	}: {
		userId: number;
	}): Promise<Tuple2<string, number>[]> => {
		var url = this.urlMapper.mapRelative(
			`/api/users/${userId}/songs-per-genre/`,
		);
		return this.httpClient.get<Tuple2<string, number>[]>(url);
	};

	public getSongLists = ({
		userId,
		query,
		paging,
		tagIds,
		sort,
		fields,
	}: {
		userId: number;
		query?: string;
		paging: PagingProperties;
		tagIds: number[];
		sort: string;
		fields?: string;
	}): Promise<PartialFindResultContract<SongListContract>> => {
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
	// userId: User ID.
	// songId: ID of the song for which to get the rating. Cannot be null.
	// callback: Callback receiving the rating. If the user has not rated the song, or if the user is not logged in, this will be "Nothing".
	public getSongRating = ({
		userId,
		songId,
	}: {
		userId: number;
		songId: number;
	}): Promise<string> => {
		if (!userId) {
			return Promise.resolve('Nothing');
		}

		var url = this.urlMapper.mapRelative(
			`/api/users/${userId}/ratedSongs/${songId}`,
		);
		return this.httpClient.get<string>(url);
	};

	public getAlbumTagSelections = ({
		albumId,
	}: {
		albumId: number;
	}): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/albumTags/${albumId}`),
		);
	};

	public getArtistTagSelections = ({
		artistId,
	}: {
		artistId: number;
	}): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/artistTags/${artistId}`),
		);
	};

	public getEventTagSelections = ({
		eventId,
	}: {
		eventId: number;
	}): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/eventTags/${eventId}`),
		);
	};

	public getEventSeriesTagSelections = ({
		seriesId,
	}: {
		seriesId: number;
	}): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			this.urlMapper.mapRelative(
				`/api/users/current/eventSeriesTags/${seriesId}`,
			),
		);
	};

	public getSongListTagSelections = ({
		songListId,
	}: {
		songListId: number;
	}): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			this.urlMapper.mapRelative(
				`/api/users/current/songListTags/${songListId}`,
			),
		);
	};

	public getSongTagSelections = ({
		songId,
	}: {
		songId: number;
	}): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/songTags/${songId}`),
		);
	};

	public refreshEntryEdit = ({
		entryType,
		entryId,
	}: {
		entryType: EntryType;
		entryId: number;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(
				`/api/users/current/refreshEntryEdit/?entryType=${EntryType[entryType]}&entryId=${entryId}`,
			),
		);
	};

	// eslint-disable-next-line no-empty-pattern
	public requestEmailVerification = ({}: {}): Promise<void> => {
		var url = this.mapUrl('/RequestEmailVerification');
		return this.httpClient.post<void>(url, undefined, {
			headers: {
				[HeaderNames.ContentType]: MediaTypes.APPLICATION_FORM_URLENCODED,
			},
		});
	};

	public updateAlbumTags = ({
		albumId,
		tags,
	}: {
		albumId: number;
		tags: TagBaseContract[];
	}): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/albumTags/${albumId}`),
			tags,
		);
	};

	// Updates artist subscription settings for an artist followed by a user.
	public updateArtistSubscription = ({
		artistId,
		emailNotifications,
		siteNotifications,
	}: {
		artistId: number;
		emailNotifications?: boolean;
		siteNotifications?: boolean;
	}): Promise<void> => {
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

	public updateArtistTags = ({
		artistId,
		tags,
	}: {
		artistId: number;
		tags: TagBaseContract[];
	}): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/artistTags/${artistId}`),
			tags,
		);
	};

	public updateComment = ({
		commentId,
		contract,
	}: {
		commentId: number;
		contract: CommentContract;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(`/api/users/profileComments/${commentId}`),
			contract,
		);
	};

	public updateEventForUser = ({
		eventId,
		associationType,
	}: {
		eventId: number;
		associationType: UserEventRelationshipType;
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative(
			`/api/users/current/events/${eventId}`,
		);
		return this.httpClient.post<void>(url, {
			associationType: UserEventRelationshipType[associationType],
		});
	};

	public updateEventTags = ({
		eventId,
		tags,
	}: {
		eventId: number;
		tags: TagBaseContract[];
	}): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/eventTags/${eventId}`),
			tags,
		);
	};

	public updateEventSeriesTags = ({
		seriesId,
		tags,
	}: {
		seriesId: number;
		tags: TagBaseContract[];
	}): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(
				`/api/users/current/eventSeriesTags/${seriesId}`,
			),
			tags,
		);
	};

	public updateSongListTags = ({
		songListId,
		tags,
	}: {
		songListId: number;
		tags: TagBaseContract[];
	}): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(
				`/api/users/current/songListTags/${songListId}`,
			),
			tags,
		);
	};

	// Updates rating score for a song.
	// songId: Id of the song to be updated.
	// rating: Song rating.
	// callback: Callback function to be executed when the operation is complete.
	public updateSongRating = ({
		songId,
		rating,
	}: {
		songId: number;
		rating: SongVoteRating;
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative(`/api/songs/${songId}/ratings`);
		return this.httpClient.post<void>(url, { rating: SongVoteRating[rating] });
	};

	public updateSongTags = ({
		songId,
		tags,
	}: {
		songId: number;
		tags: TagBaseContract[];
	}): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.put<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/songTags/${songId}`),
			tags,
		);
	};

	// Updates user setting.
	// userId: user ID.
	// settingName: name of the setting to be updated, for example 'showChatBox'.
	// value: setting value, for example 'false'.
	public updateUserSetting = ({
		userId,
		settingName,
		value,
	}: {
		userId: number;
		settingName: string;
		value: string;
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative(
			`/api/users/${userId}/settings/${settingName}`,
		);
		return this.httpClient.post<void>(url, `"${value}"`, {
			headers: { [HeaderNames.ContentType]: MediaTypes.APPLICATION_JSON },
		});
	};
}
