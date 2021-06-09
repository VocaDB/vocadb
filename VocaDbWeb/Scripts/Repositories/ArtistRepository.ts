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
import functions from '@Shared/GlobalFunctions';
import HttpClient, { HeaderNames, MediaTypes } from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
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

	private readonly urlMapper: UrlMapper;

	constructor(private readonly httpClient: HttpClient, baseUrl: string) {
		super(baseUrl);

		this.urlMapper = new UrlMapper(baseUrl);

		this.mapUrl = (relative: string): string => {
			return `${functions.mergeUrls(baseUrl, '/Artist')}${relative}`;
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
			this.urlMapper.mapRelative(`/api/artists/${artistId}/comments`),
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
			this.urlMapper.mapRelative('/Artist/CreateReport'),
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
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(`/api/artists/comments/${commentId}`),
		);
	};

	public findDuplicate: (
		params: any,
	) => Promise<DuplicateEntryResultContract[]>;

	public getComments = (artistId: number): Promise<CommentContract[]> => {
		return this.httpClient.get<CommentContract[]>(
			this.urlMapper.mapRelative(`/api/artists/${artistId}/comments`),
		);
	};

	public getForEdit = (id: number): Promise<ArtistForEditContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}/for-edit`);
		return this.httpClient.get<ArtistForEditContract>(url);
	};

	public getOne = (
		id: number,
		lang: ContentLanguagePreference,
	): Promise<ArtistContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}`);
		return this.httpClient.get<ArtistContract>(url, {
			fields: 'AdditionalNames',
			lang: ContentLanguagePreference[lang],
		});
	};

	public getOneWithComponents = (
		id: number,
		fields: string,
		lang: ContentLanguagePreference,
	): Promise<ArtistApiContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}`);
		return this.httpClient.get<ArtistApiContract>(url, {
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
		var url = functions.mergeUrls(this.baseUrl, '/api/artists');
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
			url,
			data,
		);
	};

	public getTagSuggestions = (
		artistId: number,
	): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.get<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(`/api/artists/${artistId}/tagSuggestions`),
		);
	};

	public updateComment = (
		commentId: number,
		contract: CommentContract,
	): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(`/api/artists/comments/${commentId}`),
			contract,
		);
	};
}

export interface ArtistQueryParams extends CommonQueryParams {
	artistTypes: string;
}
