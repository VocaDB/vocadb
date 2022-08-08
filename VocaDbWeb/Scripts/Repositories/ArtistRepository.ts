import ArtistApiContract from '@/DataContracts/Artist/ArtistApiContract';
import ArtistContract from '@/DataContracts/Artist/ArtistContract';
import ArtistDetailsContract from '@/DataContracts/Artist/ArtistDetailsContract';
import ArtistForEditContract from '@/DataContracts/Artist/ArtistForEditContract';
import CreateArtistContract from '@/DataContracts/Artist/CreateArtistContract';
import CommentContract from '@/DataContracts/CommentContract';
import DuplicateEntryResultContract from '@/DataContracts/DuplicateEntryResultContract';
import PagingProperties from '@/DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@/DataContracts/PartialFindResultContract';
import TagUsageForApiContract from '@/DataContracts/Tag/TagUsageForApiContract';
import EntryWithArchivedVersionsContract from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import AjaxHelper from '@/Helpers/AjaxHelper';
import ArtistType from '@/Models/Artists/ArtistType';
import ContentLanguagePreference from '@/Models/Globalization/ContentLanguagePreference';
import functions from '@/Shared/GlobalFunctions';
import HttpClient, { HeaderNames, MediaTypes } from '@/Shared/HttpClient';
import UrlMapper from '@/Shared/UrlMapper';
import AdvancedSearchFilter from '@/ViewModels/Search/AdvancedSearchFilter';

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

	public constructor(private readonly httpClient: HttpClient, baseUrl: string) {
		super(baseUrl);

		this.urlMapper = new UrlMapper(baseUrl);

		this.mapUrl = (relative: string): string => {
			return `${functions.mergeUrls(baseUrl, '/Artist')}${relative}`;
		};

		this.findDuplicate = ({
			params,
		}: {
			params: any;
		}): Promise<DuplicateEntryResultContract[]> => {
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

	public createComment = ({
		entryId: artistId,
		contract,
	}: {
		entryId: number;
		contract: CommentContract;
	}): Promise<CommentContract> => {
		return this.httpClient.post<CommentContract>(
			this.urlMapper.mapRelative(`/api/artists/${artistId}/comments`),
			contract,
		);
	};

	public createReport = ({
		artistId,
		reportType,
		notes,
		versionNumber,
	}: {
		artistId: number;
		reportType: string;
		notes: string;
		versionNumber?: number;
	}): Promise<void> => {
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

	public deleteComment = ({
		commentId,
	}: {
		commentId: number;
	}): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(`/api/artists/comments/${commentId}`),
		);
	};

	public findDuplicate: ({
		params,
	}: {
		params: any;
	}) => Promise<DuplicateEntryResultContract[]>;

	public getComments = ({
		entryId: artistId,
	}: {
		entryId: number;
	}): Promise<CommentContract[]> => {
		return this.httpClient.get<CommentContract[]>(
			this.urlMapper.mapRelative(`/api/artists/${artistId}/comments`),
		);
	};

	public getForEdit = ({
		id,
	}: {
		id: number;
	}): Promise<ArtistForEditContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}/for-edit`);
		return this.httpClient.get<ArtistForEditContract>(url);
	};

	public getOne = ({
		id,
		lang,
	}: {
		id: number;
		lang: ContentLanguagePreference;
	}): Promise<ArtistContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}`);
		return this.httpClient.get<ArtistContract>(url, {
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
	}): Promise<ArtistApiContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/artists/${id}`);
		return this.httpClient.get<ArtistApiContract>(url, {
			fields: fields,
			lang: lang,
		});
	};

	public getList = ({
		paging,
		lang,
		query,
		sort,
		artistTypes,
		allowBaseVoicebanks,
		tags,
		childTags,
		followedByUserId,
		fields,
		status,
		advancedFilters,
	}: {
		paging: PagingProperties;
		lang: ContentLanguagePreference;
		query: string;
		sort: string;
		artistTypes?: ArtistType[];
		allowBaseVoicebanks: boolean;
		tags: number[];
		childTags: boolean;
		followedByUserId?: number;
		fields: string;
		status?: string;
		advancedFilters: AdvancedSearchFilter[];
	}): Promise<PartialFindResultContract<ArtistContract>> => {
		var url = functions.mergeUrls(this.baseUrl, '/api/artists');
		var data = {
			start: paging.start,
			getTotalCount: paging.getTotalCount,
			maxResults: paging.maxEntries,
			query: query,
			fields: fields,
			lang: lang,
			nameMatchMode: 'Auto',
			sort: sort,
			artistTypes: artistTypes?.join(','),
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

	public getTagSuggestions = ({
		artistId,
	}: {
		artistId: number;
	}): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.get<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(`/api/artists/${artistId}/tagSuggestions`),
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
			this.urlMapper.mapRelative(`/api/artists/comments/${commentId}`),
			contract,
		);
	};

	public getDetails = ({
		id,
	}: {
		id: number;
	}): Promise<ArtistDetailsContract> => {
		return this.httpClient.get<ArtistDetailsContract>(
			this.urlMapper.mapRelative(`/api/artists/${id}/details`),
		);
	};

	public getArtistWithArchivedVersions = ({
		id,
	}: {
		id: number;
	}): Promise<EntryWithArchivedVersionsContract<ArtistApiContract>> => {
		return this.httpClient.get<
			EntryWithArchivedVersionsContract<ArtistApiContract>
		>(this.urlMapper.mapRelative(`/api/artists/${id}/versions`));
	};

	public create = (
		contract: CreateArtistContract,
		pictureUpload: File | undefined,
	): Promise<number> => {
		const formData = new FormData();
		formData.append('contract', JSON.stringify(contract));

		if (pictureUpload) formData.append('pictureUpload', pictureUpload);

		return this.httpClient.post<number>(
			this.urlMapper.mapRelative('/api/artists'),
			formData,
			{
				headers: {
					'Content-Type': 'multipart/form-data',
					requestVerificationToken: vdb.values.requestToken,
				},
			},
		);
	};

	public edit = (
		contract: ArtistForEditContract,
		coverPicUpload: File | undefined,
		pictureUpload: File[],
	): Promise<number> => {
		const formData = new FormData();
		formData.append('contract', JSON.stringify(contract));

		if (coverPicUpload) formData.append('coverPicUpload', coverPicUpload);

		for (const file of pictureUpload) formData.append('pictureUpload', file);

		return this.httpClient.post<number>(
			this.urlMapper.mapRelative(`/api/artists/${contract.id}`),
			formData,
			{
				headers: {
					'Content-Type': 'multipart/form-data',
					requestVerificationToken: vdb.values.requestToken,
				},
			},
		);
	};
}

export interface ArtistQueryParams extends CommonQueryParams {
	artistTypes: string /* TODO: ArtistType[] */;
}
