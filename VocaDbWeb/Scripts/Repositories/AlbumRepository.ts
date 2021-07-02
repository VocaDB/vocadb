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
import functions from '@Shared/GlobalFunctions';
import HttpClient, { HeaderNames, MediaTypes } from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
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

	private readonly urlMapper: UrlMapper;

	public constructor(private readonly httpClient: HttpClient, baseUrl: string) {
		super(baseUrl);

		this.urlMapper = new UrlMapper(baseUrl);

		this.mapUrl = (relative): string => {
			return `${functions.mergeUrls(baseUrl, '/Album')}${relative}`;
		};
	}

	public createComment = ({
		entryId: albumId,
		contract,
	}: {
		entryId: number;
		contract: CommentContract;
	}): Promise<CommentContract> => {
		return this.httpClient.post<CommentContract>(
			this.urlMapper.mapRelative(`/api/albums/${albumId}/comments`),
			contract,
		);
	};

	public createOrUpdateReview({
		albumId,
		reviewContract,
	}: {
		albumId: number;
		reviewContract: AlbumReviewContract;
	}): Promise<AlbumReviewContract> {
		const url = functions.mergeUrls(
			this.baseUrl,
			`/api/albums/${albumId}/reviews`,
		);
		return this.httpClient.post<AlbumReviewContract>(url, reviewContract);
	}

	public createReport = ({
		albumId,
		reportType,
		notes,
		versionNumber,
	}: {
		albumId: number;
		reportType: string;
		notes: string;
		versionNumber?: number;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative('/Album/CreateReport'),
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

	public deleteComment = ({
		commentId,
	}: {
		commentId: number;
	}): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(`/api/albums/comments/${commentId}`),
		);
	};

	public deleteReview({
		albumId,
		reviewId,
	}: {
		albumId: number;
		reviewId: number;
	}): Promise<void> {
		const url = functions.mergeUrls(
			this.baseUrl,
			`/api/albums/${albumId}/reviews/${reviewId}`,
		);
		return this.httpClient.delete(url);
	}

	public findDuplicate = ({
		params,
	}: {
		params: {
			term1: string;
			term2: string;
			term3: string;
		};
	}): Promise<DuplicateEntryResultContract[]> => {
		var url = functions.mergeUrls(this.baseUrl, '/Album/FindDuplicate');
		return this.httpClient.get<DuplicateEntryResultContract[]>(url, params);
	};

	public getComments = ({
		entryId: albumId,
	}: {
		entryId: number;
	}): Promise<CommentContract[]> => {
		return this.httpClient.get<CommentContract[]>(
			this.urlMapper.mapRelative(`/api/albums/${albumId}/comments`),
		);
	};

	public getForEdit = ({
		id,
	}: {
		id: number;
	}): Promise<AlbumForEditContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}/for-edit`);
		return this.httpClient.get<AlbumForEditContract>(url);
	};

	public getOne = ({
		id,
		lang,
	}: {
		id: number;
		lang: ContentLanguagePreference;
	}): Promise<AlbumContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}`);
		return this.httpClient.get<AlbumContract>(url, {
			fields: 'AdditionalNames',
			lang: lang,
		});
	};

	public getOneWithComponents = ({
		id,
		fields,
		lang,
	}: {
		id: number;
		fields: string;
		lang: ContentLanguagePreference;
	}): Promise<AlbumForApiContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}`);
		return this.httpClient.get<AlbumForApiContract>(url, {
			fields: fields,
			lang: lang,
		});
	};

	public getList = ({
		paging,
		lang,
		query,
		sort,
		discTypes,
		tags,
		childTags,
		artistIds,
		artistParticipationStatus,
		childVoicebanks,
		includeMembers,
		fields,
		status,
		deleted,
		advancedFilters,
	}: {
		paging: PagingProperties;
		lang: ContentLanguagePreference;
		query: string;
		sort: string;
		discTypes?: string;
		tags?: number[];
		childTags?: boolean;
		artistIds?: number[];
		artistParticipationStatus?: string;
		childVoicebanks?: boolean;
		includeMembers?: boolean;
		fields: string;
		status?: string;
		deleted: boolean;
		advancedFilters?: AdvancedSearchFilter[];
	}): Promise<PartialFindResultContract<AlbumContract>> => {
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

	public getReviews = ({
		albumId,
	}: {
		albumId: number;
	}): Promise<AlbumReviewContract[]> => {
		const url = functions.mergeUrls(
			this.baseUrl,
			`/api/albums/${albumId}/reviews`,
		);
		return this.httpClient.get<AlbumReviewContract[]>(url);
	};

	public getTagSuggestions = ({
		albumId,
	}: {
		albumId: number;
	}): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.get<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(`/api/albums/${albumId}/tagSuggestions`),
		);
	};

	public async getUserCollections({
		albumId,
	}: {
		albumId: number;
	}): Promise<AlbumForUserForApiContract[]> {
		const url = functions.mergeUrls(
			this.baseUrl,
			`/api/albums/${albumId}/user-collections`,
		);
		return this.httpClient.get<AlbumForUserForApiContract[]>(url);
	}

	public updateComment = ({
		commentId,
		contract,
	}: {
		commentId: number;
		contract: CommentContract;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(`/api/albums/comments/${commentId}`),
			contract,
		);
	};

	public updatePersonalDescription = ({
		albumId,
		text,
		author,
	}: {
		albumId: number;
		text: string;
		author: ArtistContract;
	}): Promise<void> => {
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
