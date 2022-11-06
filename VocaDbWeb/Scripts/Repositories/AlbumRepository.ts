import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { AlbumDetailsContract } from '@/DataContracts/Album/AlbumDetailsContract';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { AlbumForEditContract } from '@/DataContracts/Album/AlbumForEditContract';
import { AlbumReviewContract } from '@/DataContracts/Album/AlbumReviewContract';
import { ArchivedAlbumVersionDetailsContract } from '@/DataContracts/Album/ArchivedAlbumVersionDetailsContract';
import { CreateAlbumContract } from '@/DataContracts/Album/CreateAlbumContract';
import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { CommentContract } from '@/DataContracts/CommentContract';
import { DuplicateEntryResultContract } from '@/DataContracts/DuplicateEntryResultContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { SongInAlbumContract } from '@/DataContracts/Song/SongInAlbumContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { AlbumForUserForApiContract } from '@/DataContracts/User/AlbumForUserForApiContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { AjaxHelper } from '@/Helpers/AjaxHelper';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import {
	BaseRepository,
	CommonQueryParams,
} from '@/Repositories/BaseRepository';
import { ICommentRepository } from '@/Repositories/ICommentRepository';
import { SongOptionalField } from '@/Repositories/SongRepository';
import { functions } from '@/Shared/GlobalFunctions';
import { HeaderNames, HttpClient, MediaTypes } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';
import qs from 'qs';

export enum AlbumOptionalField {
	AdditionalNames = 'AdditionalNames',
	Artists = 'Artists',
	Description = 'Description',
	Discs = 'Discs',
	Identifiers = 'Identifiers',
	MainPicture = 'MainPicture',
	Names = 'Names',
	PVs = 'PVs',
	ReleaseEvent = 'ReleaseEvent',
	Tags = 'Tags',
	Tracks = 'Tracks',
	WebLinks = 'WebLinks',
}

// Repository for managing albums and related objects.
// Corresponds to the AlbumController class.
export class AlbumRepository
	extends BaseRepository
	implements ICommentRepository {
	// Maps a relative URL to an absolute one.
	private mapUrl: (relative: string) => string;

	private readonly urlMapper: UrlMapper;

	constructor(private readonly httpClient: HttpClient, baseUrl: string) {
		super(baseUrl);

		this.urlMapper = new UrlMapper(baseUrl);

		this.mapUrl = (relative): string => {
			return `${functions.mergeUrls(baseUrl, '/Album')}${relative}`;
		};
	}

	createComment = ({
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

	createOrUpdateReview({
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

	createReport = ({
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

	deleteComment = ({ commentId }: { commentId: number }): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(`/api/albums/comments/${commentId}`),
		);
	};

	deleteReview({
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

	findDuplicate = ({
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

	getComments = ({
		entryId: albumId,
	}: {
		entryId: number;
	}): Promise<CommentContract[]> => {
		return this.httpClient.get<CommentContract[]>(
			this.urlMapper.mapRelative(`/api/albums/${albumId}/comments`),
		);
	};

	getForEdit = ({ id }: { id: number }): Promise<AlbumForEditContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}/for-edit`);
		return this.httpClient.get<AlbumForEditContract>(url);
	};

	getOne = ({
		id,
		lang,
	}: {
		id: number;
		lang: ContentLanguagePreference;
	}): Promise<AlbumContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}`);
		return this.httpClient.get<AlbumContract>(url, {
			fields: AlbumOptionalField.AdditionalNames,
			lang: lang,
		});
	};

	getOneWithComponents = ({
		id,
		fields,
		lang,
		songFields,
	}: {
		id: number;
		fields: AlbumOptionalField[];
		lang: ContentLanguagePreference;
		songFields?: SongOptionalField[];
	}): Promise<AlbumForApiContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/albums/${id}`);
		return this.httpClient.get<AlbumForApiContract>(url, {
			fields: fields.join(','),
			lang: lang,
			songFields: songFields?.join(','),
		});
	};

	getList = ({
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
		discTypes?: AlbumType[];
		tags?: number[];
		childTags?: boolean;
		artistIds?: number[];
		artistParticipationStatus?: string;
		childVoicebanks?: boolean;
		includeMembers?: boolean;
		fields: AlbumOptionalField[];
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
			fields: fields.join(','),
			lang: lang,
			nameMatchMode: 'Auto',
			sort: sort,
			discTypes: discTypes?.join(','),
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

	getReviews = ({
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

	getTagSuggestions = ({
		albumId,
	}: {
		albumId: number;
	}): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.get<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(`/api/albums/${albumId}/tagSuggestions`),
		);
	};

	getTracks = ({
		id,
		fields,
		lang,
	}: {
		id: number;
		fields: SongOptionalField[];
		lang: ContentLanguagePreference;
	}): Promise<SongInAlbumContract[]> => {
		return this.httpClient.get<SongInAlbumContract[]>(
			this.urlMapper.mapRelative(`/api/albums/${id}/tracks`),
			{
				fields: fields.join(','),
				lang: lang,
			},
		);
	};

	async getUserCollections({
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

	updateComment = ({
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

	updatePersonalDescription = ({
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

	getDetails = ({ id }: { id: number }): Promise<AlbumDetailsContract> => {
		return this.httpClient.get<AlbumDetailsContract>(
			this.urlMapper.mapRelative(`/api/albums/${id}/details`),
		);
	};

	getAlbumWithArchivedVersions = ({
		id,
	}: {
		id: number;
	}): Promise<EntryWithArchivedVersionsContract<AlbumForApiContract>> => {
		return this.httpClient.get<
			EntryWithArchivedVersionsContract<AlbumForApiContract>
		>(this.urlMapper.mapRelative(`/api/albums/${id}/versions`));
	};

	getVersionDetails = ({
		id,
		comparedVersionId,
	}: {
		id: number;
		comparedVersionId?: number;
	}): Promise<ArchivedAlbumVersionDetailsContract> => {
		return this.httpClient.get<ArchivedAlbumVersionDetailsContract>(
			this.urlMapper.mapRelative(`/api/albums/versions/${id}`),
			{ comparedVersionId: comparedVersionId },
		);
	};

	create = (
		requestToken: string,
		contract: CreateAlbumContract,
	): Promise<number> => {
		const formData = new FormData();
		formData.append('contract', JSON.stringify(contract));

		return this.httpClient.post<number>(
			this.urlMapper.mapRelative('/api/albums'),
			formData,
			{
				headers: {
					'Content-Type': 'multipart/form-data',
					requestVerificationToken: requestToken,
				},
			},
		);
	};

	edit = (
		requestToken: string,
		contract: AlbumForEditContract,
		coverPicUpload: File | undefined,
		pictureUpload: File[],
	): Promise<number> => {
		const formData = new FormData();
		formData.append('contract', JSON.stringify(contract));

		if (coverPicUpload) formData.append('coverPicUpload', coverPicUpload);

		for (const file of pictureUpload) formData.append('pictureUpload', file);

		return this.httpClient.post<number>(
			this.urlMapper.mapRelative(`/api/albums/${contract.id}`),
			formData,
			{
				headers: {
					'Content-Type': 'multipart/form-data',
					requestVerificationToken: requestToken,
				},
			},
		);
	};

	merge = (
		requestToken: string,
		{ id, targetAlbumId }: { id: number; targetAlbumId: number },
	): Promise<void> => {
		return this.httpClient.post(
			this.urlMapper.mapRelative(
				`/api/albums/${id}/merge?${qs.stringify({
					targetAlbumId: targetAlbumId,
				})}`,
			),
			undefined,
			{
				headers: {
					requestVerificationToken: requestToken,
				},
			},
		);
	};
}

export interface AlbumQueryParams extends CommonQueryParams {
	discTypes: AlbumType[];
}
