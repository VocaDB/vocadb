import { CommentContract } from '@/DataContracts/CommentContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { SongListContract } from '@/DataContracts/Song/SongListContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagSelectionContract } from '@/DataContracts/Tag/TagSelectionContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import {
	AlbumForUserForApiContract,
	MediaType,
} from '@/DataContracts/User/AlbumForUserForApiContract';
import { ArtistForUserForApiContract } from '@/DataContracts/User/ArtistForUserForApiContract';
import { EntryEditDataContract } from '@/DataContracts/User/EntryEditDataContract';
import { RatedSongForUserForApiContract } from '@/DataContracts/User/RatedSongForUserForApiContract';
import { UpdateUserSettingsContract } from '@/DataContracts/User/UpdateUserSettingsContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { UserDetailsContract } from '@/DataContracts/User/UserDetailsContract';
import { UserForEditContract } from '@/DataContracts/User/UserForEditContract';
import { UserForMySettingsContract } from '@/DataContracts/User/UserForMySettingsContract';
import { UserMessageSummaryContract } from '@/DataContracts/User/UserMessageSummaryContract';
import { UserWithPermissionsForApiContract } from '@/DataContracts/User/UserWithPermissionsForApiContract';
import { AjaxHelper } from '@/Helpers/AjaxHelper';
import { Tuple2 } from '@/Helpers/HighchartsHelper';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { EntryType } from '@/Models/EntryType';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { NameMatchMode } from '@/Models/NameMatchMode';
import { PVService } from '@/Models/PVs/PVService';
import { SongVoteRating } from '@/Models/SongVoteRating';
import { UserEventRelationshipType } from '@/Models/Users/UserEventRelationshipType';
import { UserGroup } from '@/Models/Users/UserGroup';
import { AlbumOptionalField } from '@/Repositories/AlbumRepository';
import { ArtistOptionalField } from '@/Repositories/ArtistRepository';
import { ICommentRepository } from '@/Repositories/ICommentRepository';
import { SongListOptionalField } from '@/Repositories/SongListRepository';
import { SongOptionalField } from '@/Repositories/SongRepository';
import {
	HeaderNames,
	HttpClient,
	httpClient,
	MediaTypes,
} from '@/Shared/HttpClient';
import { UrlMapper, urlMapper } from '@/Shared/UrlMapper';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';

export enum UserInboxType {
	Received = 'Received',
	Sent = 'Sent',
	Notifications = 'Notifications',
}

export interface UserGetRatedSongsListQueryParams {
	userId: number;
	query: string;
	tagIds: number[];
	artistIds: number[];
	childVoicebanks: boolean;
	rating: string;
	songListId?: number;
	advancedFilters: AdvancedSearchFilter[];
	groupByRating: boolean;
	sort: string;
}

export enum UserOptionalField {
	'KnownLanguages' = 'KnownLanguages',
	'MainPicture' = 'MainPicture',
	'OldUsernames' = 'OldUsernames',
}

// Repository for managing users and related objects.
// Corresponds to the UserController class.
export class UserRepository implements ICommentRepository {
	// Maps a relative URL to an absolute one.
	private mapUrl: (relative: string) => string;

	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {
		this.mapUrl = (relative: string): string => {
			return `${urlMapper.mapRelative('/User')}${relative}`;
		};
	}

	addFollowedTag = ({ tagId }: { tagId: number }): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(`/api/users/current/followedTags/${tagId}`),
		);
	};

	createArtistSubscription = ({
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

	createComment = ({
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

	createMessage = ({
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

	deleteArtistSubscription = ({
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

	deleteComment = ({ commentId }: { commentId: number }): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(`/api/users/profileComments/${commentId}`),
		);
	};

	deleteEventForUser = ({ eventId }: { eventId: number }): Promise<void> => {
		var url = this.urlMapper.mapRelative(
			`/api/users/current/events/${eventId}`,
		);
		return this.httpClient.delete<void>(url);
	};

	deleteFollowedTag = ({ tagId }: { tagId: number }): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(`/api/users/current/followedTags/${tagId}`),
		);
	};

	deleteMessage = ({ messageId }: { messageId: number }): Promise<void> => {
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

	deleteMessages = ({
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

	getAlbumCollectionList = ({
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
		mediaType,
	}: {
		userId: number;
		paging: PagingProperties;
		lang: ContentLanguagePreference;
		query: string;
		tag?: number;
		albumType: AlbumType;
		artistId?: number;
		purchaseStatuses: string;
		releaseEventId?: number;
		advancedFilters: AdvancedSearchFilter[];
		sort: string;
		mediaType?: MediaType;
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
			fields: [
				AlbumOptionalField.AdditionalNames,
				AlbumOptionalField.MainPicture,
				AlbumOptionalField.ReleaseEvent,
			].join(','),
			lang: lang,
			nameMatchMode: 'Auto',
			sort: sort,
			advancedFilters: advancedFilters,
			mediaType: mediaType,
		};

		return this.httpClient.get<
			PartialFindResultContract<AlbumForUserForApiContract>
		>(url, data);
	};

	getComments = async ({
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

	getEvents = ({
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

	getFollowedArtistsList = ({
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
		artistType: ArtistType;
	}): Promise<PartialFindResultContract<ArtistForUserForApiContract>> => {
		var url = this.urlMapper.mapRelative(
			`/api/users/${userId}/followedArtists`,
		);
		var data = {
			start: paging.start,
			getTotalCount: paging.getTotalCount,
			maxResults: paging.maxEntries,
			tagId: tagIds,
			fields: [
				ArtistOptionalField.AdditionalNames,
				ArtistOptionalField.MainPicture,
			].join(','),
			lang: lang,
			nameMatchMode: 'Auto',
			artistType: artistType,
		};

		return this.httpClient.get<
			PartialFindResultContract<ArtistForUserForApiContract>
		>(url, data);
	};

	getList = ({
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
		paging?: PagingProperties;
		query: string;
		sort?: string;
		groups?: UserGroup;
		includeDisabled: boolean;
		onlyVerified: boolean;
		knowsLanguage?: string;
		nameMatchMode: NameMatchMode;
		fields?: UserOptionalField[];
	}): Promise<PartialFindResultContract<UserApiContract>> => {
		var url = this.urlMapper.mapRelative('/api/users');
		var data = {
			start: paging?.start,
			getTotalCount: paging?.getTotalCount,
			maxResults: paging?.maxEntries,
			query: query,
			nameMatchMode: nameMatchMode,
			sort: sort,
			includeDisabled: includeDisabled,
			onlyVerified: onlyVerified,
			knowsLanguage: knowsLanguage,
			groups: groups || undefined,
			fields: fields?.join(','),
		};

		return this.httpClient.get<PartialFindResultContract<UserApiContract>>(
			url,
			data,
		);
	};

	getOne = ({
		id,
		fields,
	}: {
		id: number;
		fields?: UserOptionalField[];
	}): Promise<UserApiContract> => {
		var url = this.urlMapper.mapRelative(`/api/users/${id}`);
		return this.httpClient.get<UserApiContract>(url, {
			fields: fields?.join(','),
		});
	};

	getOneByName = async ({
		username,
	}: {
		username: string;
	}): Promise<UserApiContract | undefined> => {
		const result = await this.getList({
			query: username,
			sort: undefined,
			groups: undefined,
			includeDisabled: false,
			onlyVerified: false,
			knowsLanguage: undefined,
			nameMatchMode: NameMatchMode.Exact,
			fields: undefined,
		});
		return result.items.length === 1 ? result.items[0] : undefined;
	};

	getMessage = ({
		messageId,
	}: {
		messageId: number;
	}): Promise<UserMessageSummaryContract> => {
		var url = this.urlMapper.mapRelative(`/api/users/messages/${messageId}`);
		return this.httpClient.get<UserMessageSummaryContract>(url);
	};

	getMessageSummaries = ({
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

	getRatedSongsList = ({
		fields,
		lang,
		paging,
		pvServices,
		queryParams,
	}: {
		fields: SongOptionalField[];
		lang: ContentLanguagePreference;
		paging: PagingProperties;
		pvServices?: PVService[];
		queryParams: UserGetRatedSongsListQueryParams;
	}): Promise<PartialFindResultContract<RatedSongForUserForApiContract>> => {
		const {
			userId,
			query,
			tagIds,
			artistIds,
			childVoicebanks,
			rating,
			songListId,
			advancedFilters,
			groupByRating,
			sort,
		} = queryParams;

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
			pvServices: pvServices?.join(','),
			fields: fields.join(','),
			lang: lang,
			nameMatchMode: 'Auto',
			sort: sort,
		};

		return this.httpClient.get<
			PartialFindResultContract<RatedSongForUserForApiContract>
		>(url, data);
	};

	getRatingsByGenre = ({
		userId,
	}: {
		userId: number;
	}): Promise<Tuple2<string, number>[]> => {
		var url = this.urlMapper.mapRelative(
			`/api/users/${userId}/songs-per-genre/`,
		);
		return this.httpClient.get<Tuple2<string, number>[]>(url);
	};

	getSongLists = ({
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
		fields?: SongListOptionalField[];
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
				fields: fields?.join(','),
			},
		);
	};

	// Gets a specific user's rating for a specific song.
	// userId: User ID.
	// songId: ID of the song for which to get the rating. Cannot be null.
	// callback: Callback receiving the rating. If the user has not rated the song, or if the user is not logged in, this will be "Nothing".
	getSongRating = ({
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

	getAlbumTagSelections = ({
		albumId,
	}: {
		albumId: number;
	}): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/albumTags/${albumId}`),
		);
	};

	getArtistTagSelections = ({
		artistId,
	}: {
		artistId: number;
	}): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/artistTags/${artistId}`),
		);
	};

	getEventTagSelections = ({
		eventId,
	}: {
		eventId: number;
	}): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/eventTags/${eventId}`),
		);
	};

	getEventSeriesTagSelections = ({
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

	getSongListTagSelections = ({
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

	getSongTagSelections = ({
		songId,
	}: {
		songId: number;
	}): Promise<TagSelectionContract[]> => {
		return this.httpClient.get<TagSelectionContract[]>(
			this.urlMapper.mapRelative(`/api/users/current/songTags/${songId}`),
		);
	};

	refreshEntryEdit = ({
		entryType,
		entryId,
	}: {
		entryType: EntryType;
		entryId: number;
	}): Promise<EntryEditDataContract> => {
		return this.httpClient.post<EntryEditDataContract>(
			this.urlMapper.mapRelative(
				`/api/users/current/refreshEntryEdit/?entryType=${entryType}&entryId=${entryId}`,
			),
		);
	};

	// eslint-disable-next-line no-empty-pattern
	requestEmailVerification = ({}: {}): Promise<void> => {
		var url = this.mapUrl('/RequestEmailVerification');
		return this.httpClient.post<void>(url, undefined, {
			headers: {
				[HeaderNames.ContentType]: MediaTypes.APPLICATION_FORM_URLENCODED,
			},
		});
	};

	updateAlbumTags = ({
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
	updateArtistSubscription = ({
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

	updateArtistTags = ({
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

	updateComment = ({
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

	setCommentsLocked = ({
		entryId,
		locked,
	}: {
		entryId: number;
		locked: boolean;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(
				`/api/comments/User-comments/${entryId}/locked`,
			),
			locked,
			{ headers: { [HeaderNames.ContentType]: MediaTypes.APPLICATION_JSON } },
		);
	};

	updateEventForUser = ({
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

	updateEventTags = ({
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

	updateEventSeriesTags = ({
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

	updateSongListTags = ({
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
	updateSongRating = ({
		songId,
		rating,
	}: {
		songId: number;
		rating: SongVoteRating;
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative(`/api/songs/${songId}/ratings`);
		return this.httpClient.post<void>(url, { rating: SongVoteRating[rating] });
	};

	updateSongTags = ({
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

	deleteTag = ({
		entryType,
		tagUsageId,
	}: {
		entryType: EntryType;
		tagUsageId: number;
	}): Promise<void> => {
		return this.httpClient.delete(
			this.urlMapper.mapRelative(
				`/api/users/current/${entryType}Tags/${tagUsageId}`,
			),
		);
	};

	// Updates user setting.
	// userId: user ID.
	// settingName: name of the setting to be updated, for example 'showChatBox'.
	// value: setting value, for example 'false'.
	updateUserSetting = ({
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
		return this.httpClient.post<void>(url, `${value}`, {
			headers: { [HeaderNames.ContentType]: MediaTypes.APPLICATION_JSON },
		});
	};

	getDetails = ({ name }: { name: string }): Promise<UserDetailsContract> => {
		return this.httpClient.get<UserDetailsContract>(
			this.urlMapper.mapRelative(`/api/profiles/${name}`),
		);
	};

	getForMySettings = (): Promise<UserForMySettingsContract> => {
		return this.httpClient.get<UserForMySettingsContract>(
			this.urlMapper.mapRelative('/api/users/current/for-my-settings'),
		);
	};

	updateMySettings = (
		requestToken: string,
		contract: UpdateUserSettingsContract,
		pictureUpload: File | undefined,
	): Promise<string> => {
		const formData = new FormData();
		formData.append('contract', JSON.stringify(contract));

		if (pictureUpload) formData.append('pictureUpload', pictureUpload);

		return this.httpClient.post<string>(
			this.urlMapper.mapRelative('/api/users/current/my-settings'),
			formData,
			{
				headers: {
					'Content-Type': 'multipart/form-data',
					requestVerificationToken: requestToken,
				},
			},
		);
	};

	getForEdit = ({
		id,
	}: {
		id: number;
	}): Promise<UserWithPermissionsForApiContract> => {
		return this.httpClient.get<UserWithPermissionsForApiContract>(
			this.urlMapper.mapRelative(`/api/users/${id}/for-edit`),
		);
	};

	create = (
		requestToken: string,
		{
			email,
			entryTime,
			extra,
			password,
			recaptchaResponse,
			userName,
		}: {
			email: string;
			entryTime: Date;
			extra: string;
			password: string;
			recaptchaResponse: string;
			userName: string;
		},
	): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative('/api/users/register'),
			{
				email: email,
				// https://stackoverflow.com/questions/7966559/how-to-convert-javascript-date-object-to-ticks/7968483#7968483
				entryTime: entryTime.getTime() * 10000 + 621355968000000000,
				extra: extra,
				'g-recaptcha-response': recaptchaResponse,
				password: password,
				userName: userName,
			},
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	edit = (
		requestToken: string,
		contract: UserForEditContract,
	): Promise<number> => {
		return this.httpClient.post<number>(
			this.urlMapper.mapRelative(`/api/users/${contract.id}`),
			contract,
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	login = (
		requestToken: string,
		{
			keepLoggedIn,
			password,
			userName,
		}: {
			keepLoggedIn: boolean;
			password: string;
			userName: string;
		},
	): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative('/api/users/login'),
			{ keepLoggedIn: keepLoggedIn, password: password, userName: userName },
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	logout = (requestToken: string): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative('/api/users/logout'),
			undefined,
			{
				headers: {
					requestVerificationToken: requestToken,
				},
			},
		);
	};

	postStatusLimited = (
		requestToken: string,
		{
			id,
			notes,
		}: {
			id: number;
			notes: string;
		},
	): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(`/api/users/${id}/status-limited`),
			{ reason: notes, createReport: true },
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	postReport = (
		requestToken: string,
		{ id, notes }: { id: number; notes: string },
	): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(`/api/users/${id}/reports`),
			{ reason: notes, reportType: 'Spamming' },
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	forgotPassword = (
		requestToken: string,
		{
			email,
			recaptchaResponse,
			username,
		}: {
			email: string;
			recaptchaResponse: string;
			username: string;
		},
	): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative('/api/users/forgot-password'),
			{
				email: email,
				'g-recaptcha-response': recaptchaResponse,
				username: username,
			},
			{ headers: { requestVerificationToken: requestToken } },
		);
	};
}

export const userRepo = new UserRepository(httpClient, urlMapper);
