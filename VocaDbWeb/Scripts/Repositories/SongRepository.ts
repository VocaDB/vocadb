import { CountPerDayContract } from '@/DataContracts/Aggregate/CountPerDayContract';
import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { EntryWithTagUsagesForApiContract } from '@/DataContracts/Base/EntryWithTagUsagesForApiContract';
import { CommentContract } from '@/DataContracts/CommentContract';
import { NewSongCheckResultContract } from '@/DataContracts/NewSongCheckResultContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ArchivedSongVersionDetailsContract } from '@/DataContracts/Song/ArchivedSongVersionDetailsContract';
import { CreateSongContract } from '@/DataContracts/Song/CreateSongContract';
import { LyricsForSongContract } from '@/DataContracts/Song/LyricsForSongContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { SongContract } from '@/DataContracts/Song/SongContract';
import { SongDetailsContract } from '@/DataContracts/Song/SongDetailsContract';
import { SongForEditContract } from '@/DataContracts/Song/SongForEditContract';
import { SongWithPVPlayerAndVoteContract } from '@/DataContracts/Song/SongWithPVPlayerAndVoteContract';
import { SongListBaseContract } from '@/DataContracts/SongListBaseContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { RatedSongForUserForApiContract } from '@/DataContracts/User/RatedSongForUserForApiContract';
import { EntryWithArchivedVersionsContract } from '@/DataContracts/Versioning/EntryWithArchivedVersionsForApiContract';
import { AjaxHelper } from '@/Helpers/AjaxHelper';
import { TimeUnit } from '@/Models/Aggregate/TimeUnit';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVService } from '@/Models/PVs/PVService';
import { SongVoteRating } from '@/Models/SongVoteRating';
import { SongType } from '@/Models/Songs/SongType';
import {
	BaseRepository,
	CommonQueryParams,
} from '@/Repositories/BaseRepository';
import { ICommentRepository } from '@/Repositories/ICommentRepository';
import { functions } from '@/Shared/GlobalFunctions';
import {
	HeaderNames,
	httpClient,
	HttpClient,
	MediaTypes,
} from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';
import { SongSortRule } from '@/Stores/Search/SongSearchStore';
import { vdbConfig } from '@/vdbConfig';
import qs from 'qs';

export enum SongOptionalField {
	AdditionalNames = 'AdditionalNames',
	Albums = 'Albums',
	Artists = 'Artists',
	Names = 'Names',
	PVs = 'PVs',
	Tags = 'Tags',
	ThumbUrl = 'ThumbUrl',
	WebLinks = 'WebLinks',
	MainPicture = 'MainPicture',
}

export interface SongGetListQueryParams {
	query: string;
	sort: SongSortRule;
	songTypes?: SongType[];
	afterDate?: Date;
	beforeDate?: Date;
	tagIds?: number[];
	childTags?: boolean;
	unifyTypesAndTags?: boolean;
	artistIds?: number[];
	artistParticipationStatus?: string;
	childVoicebanks?: boolean;
	includeMembers?: boolean;
	eventId?: number;
	onlyWithPvs?: boolean;
	since?: number;
	minScore?: number;
	userCollectionId?: number;
	parentSongId?: number;
	status?: string;
	advancedFilters?: AdvancedSearchFilter[];
	minMilliBpm?: number;
	maxMilliBpm?: number;
	minLength?: number;
	maxLength?: number;
}

// Repository for managing songs and related objects.
// Corresponds to the SongController class.
export class SongRepository
	extends BaseRepository
	implements ICommentRepository {
	private readonly urlMapper: UrlMapper;

	constructor(private readonly httpClient: HttpClient, baseUrl: string) {
		super(baseUrl);

		this.urlMapper = new UrlMapper(baseUrl);

		this.get = <T>(relative: string, params: any): Promise<T> => {
			return this.httpClient.get<T>(this.mapUrl(relative), params);
		};

		this.getJSON = <T>(relative: string, params: any): Promise<T> => {
			return this.httpClient.get<T>(this.mapUrl(relative), params);
		};

		this.mapUrl = (relative: string): string => {
			return `${functions.mergeUrls(baseUrl, '/Song')}${relative}`;
		};

		this.post = <T>(relative: string, params: any): Promise<T> => {
			return this.httpClient.post<T>(
				this.mapUrl(relative),
				AjaxHelper.stringify(params),
				{
					headers: {
						[HeaderNames.ContentType]: MediaTypes.APPLICATION_FORM_URLENCODED,
					},
				},
			);
		};

		this.pvForSongAndService = ({
			songId,
			pvService,
		}: {
			songId: number;
			pvService: PVService;
		}): Promise<string> => {
			return this.get<string>('/PVForSongAndService', {
				songId: songId,
				service: PVService[pvService],
			});
		};

		this.songListsForSong = ({
			songId,
		}: {
			songId: number;
		}): Promise<string> => {
			return this.get<string>('/SongListsForSong', { songId: songId });
		};

		this.songListsForUser = ({
			ignoreSongId,
		}: {
			ignoreSongId: number;
		}): Promise<SongListBaseContract[]> => {
			return this.post<SongListBaseContract[]>('/SongListsForUser', {
				ignoreSongId: ignoreSongId,
			});
		};
	}

	addSongToList = ({
		listId,
		songId,
		notes,
		newListName,
	}: {
		listId: number;
		songId: number;
		notes: string;
		newListName: string;
	}): Promise<void> => {
		return this.post<void>('/AddSongToList', {
			listId: listId,
			songId: songId,
			notes: notes,
			newListName: newListName,
		});
	};

	createComment = ({
		entryId: songId,
		contract,
	}: {
		entryId: number;
		contract: CommentContract;
	}): Promise<CommentContract> => {
		return this.httpClient.post<CommentContract>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/comments`),
			contract,
		);
	};

	createReport = ({
		songId,
		reportType,
		notes,
		versionNumber,
	}: {
		songId: number;
		reportType: string;
		notes: string;
		versionNumber?: number;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative('/Song/CreateReport'),
			AjaxHelper.stringify({
				reportType: reportType,
				notes: notes,
				songId: songId,
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
			this.urlMapper.mapRelative(`/api/songs/comments/${commentId}`),
		);
	};

	findDuplicate = ({
		params,
	}: {
		params: {
			term: string[];
			pv: string[];
			artistIds: number[];
			getPVInfo: boolean;
		};
	}): Promise<NewSongCheckResultContract> => {
		return this.httpClient.get<NewSongCheckResultContract>(
			this.urlMapper.mapRelative('/api/songs/findDuplicate'),
			params,
		);
	};

	private get: <T>(relative: string, params: any) => Promise<T>;

	getByNames({
		names,
		ignoreIds,
		lang,
		songTypes,
	}: {
		names: string[];
		ignoreIds: number[];
		lang: ContentLanguagePreference;
		songTypes?: SongType[];
	}): Promise<SongApiContract[]> {
		const url = functions.mergeUrls(this.baseUrl, '/api/songs/by-names');
		return this.httpClient.get<SongApiContract[]>(url, {
			names: names,
			songTypes: songTypes,
			lang: lang,
			ignoreIds: ignoreIds,
		});
	}

	getByPV = ({
		pvService,
		pvId,
		fields,
	}: {
		pvService: PVService;
		pvId: string;
		fields?: SongOptionalField[];
	}): Promise<SongApiContract | undefined> => {
		return this.httpClient.get<SongApiContract | undefined>(
			this.urlMapper.mapRelative('/api/songs/byPv'),
			{
				pvService: pvService,
				pvId: pvId,
				fields: fields?.join(','),
			},
		);
	};

	getComments = ({
		entryId: songId,
	}: {
		entryId: number;
	}): Promise<CommentContract[]> => {
		return this.httpClient.get<CommentContract[]>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/comments`),
		);
	};

	getForEdit = ({ id }: { id: number }): Promise<SongForEditContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/songs/${id}/for-edit`);
		return this.httpClient.get<SongForEditContract>(url);
	};

	getLyrics = ({
		lyricsId,
		songVersion,
	}: {
		lyricsId: number;
		songVersion: number;
	}): Promise<LyricsForSongContract> => {
		return this.httpClient.get<LyricsForSongContract>(
			this.urlMapper.mapRelative(
				`/api/songs/lyrics/${lyricsId}?v=${songVersion}`,
			),
		);
	};

	private getJSON: <T>(relative: string, params: any) => Promise<T>;

	getOneWithComponents = ({
		baseUrl,
		id,
		fields,
		lang,
	}: {
		baseUrl?: string;
		id: number;
		fields?: SongOptionalField[];
		lang: ContentLanguagePreference;
	}): Promise<SongApiContract> => {
		var url = functions.mergeUrls(baseUrl ?? this.baseUrl, `/api/songs/${id}`);
		return this.httpClient.get<SongApiContract>(url, {
			fields: fields?.join(','),
			lang: lang,
		});
	};

	getOne = ({
		id,
		lang,
	}: {
		id: number;
		lang: ContentLanguagePreference;
	}): Promise<SongContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/songs/${id}`);
		return this.httpClient.get<SongContract>(url, {
			fields: [SongOptionalField.AdditionalNames].join(','),
			lang: lang,
		});
	};

	getListByParams({
		params,
	}: {
		params: SongQueryParams;
	}): Promise<PartialFindResultContract<SongApiContract>> {
		const url = functions.mergeUrls(this.baseUrl, '/api/songs');
		return this.httpClient.get<PartialFindResultContract<SongApiContract>>(
			url,
			params,
		);
	}

	getList = ({
		fields,
		lang,
		paging,
		pvServices,
		queryParams,
	}: {
		fields?: SongOptionalField[];
		lang: ContentLanguagePreference;
		paging: PagingProperties;
		pvServices?: PVService[];
		queryParams: SongGetListQueryParams;
	}): Promise<PartialFindResultContract<SongApiContract>> => {
		const {
			query,
			sort,
			songTypes,
			afterDate,
			beforeDate,
			tagIds,
			childTags,
			unifyTypesAndTags,
			artistIds,
			artistParticipationStatus,
			childVoicebanks,
			includeMembers,
			eventId,
			onlyWithPvs,
			since,
			minScore,
			userCollectionId,
			parentSongId,
			status,
			advancedFilters,
			minMilliBpm,
			maxMilliBpm,
			minLength,
			maxLength,
		} = queryParams;

		var url = functions.mergeUrls(this.baseUrl, '/api/songs');
		var data = {
			start: paging.start,
			getTotalCount: paging.getTotalCount,
			maxResults: paging.maxEntries,
			query: query,
			fields: fields?.join(','),
			lang: lang,
			nameMatchMode: 'Auto',
			sort: sort,
			songTypes: songTypes?.join(','),
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
			pvServices: pvServices?.join(','),
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

		return this.httpClient.get<PartialFindResultContract<SongContract>>(
			url,
			data,
		);
	};

	getOverTime = ({
		timeUnit,
		artistId,
	}: {
		timeUnit: TimeUnit;
		artistId: number;
	}): Promise<CountPerDayContract[]> => {
		var url = this.urlMapper.mapRelative('/api/songs/over-time');
		return this.httpClient.get<CountPerDayContract[]>(url, {
			timeUnit: TimeUnit[timeUnit],
			artistId: artistId,
		});
	};

	// Get PV ID by song ID and PV service.
	getPvId = ({
		songId,
		pvService,
	}: {
		songId: number;
		pvService: PVService;
	}): Promise<string> => {
		return this.httpClient.get<string>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/pvs`),
			{ service: PVService[pvService] },
		);
	};

	getRatings = ({
		songId,
	}: {
		songId: number;
	}): Promise<RatedSongForUserForApiContract[]> => {
		return this.httpClient.get<RatedSongForUserForApiContract[]>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/ratings`),
			{ userFields: 'MainPicture' },
		);
	};

	getTagSuggestions = ({
		songId,
	}: {
		songId: number;
	}): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.get<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/tagSuggestions`),
		);
	};

	getTagUsages = ({
		songId,
	}: {
		songId: number;
	}): Promise<EntryWithTagUsagesForApiContract> => {
		return this.httpClient.get<EntryWithTagUsagesForApiContract>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/tagUsages`),
		);
	};

	// Maps a relative URL to an absolute one.
	private mapUrl: (relative: string) => string;

	private post: <T>(relative: string, params: any) => Promise<T>;

	pvForSongAndService: ({
		songId,
		pvService,
	}: {
		songId: number;
		pvService: PVService;
	}) => Promise<string>;

	pvPlayer = ({
		songId,
		params,
	}: {
		songId: number;
		params: PVEmbedParams;
	}): Promise<SongWithPVPlayerAndVoteContract> => {
		return this.getJSON<SongWithPVPlayerAndVoteContract>(
			`/PVPlayer/${songId}`,
			params,
		);
	};

	//songListsForSong: (songId: number, callback: (result: SongListContract[]) => void) => void;

	songListsForSong: ({ songId }: { songId: number }) => Promise<string>;

	songListsForUser: ({
		ignoreSongId,
	}: {
		ignoreSongId: number;
	}) => Promise<SongListBaseContract[]>;

	updateComment = ({
		commentId,
		contract,
	}: {
		commentId: number;
		contract: CommentContract;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(`/api/songs/comments/${commentId}`),
			contract,
		);
	};

	updatePersonalDescription = ({
		songId,
		text,
		author,
	}: {
		songId: number;
		text?: string;
		author?: ArtistContract;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/personal-description/`),
			{
				personalDescriptionText: text,
				personalDescriptionAuthor: author || undefined,
			},
		);
	};

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

	getDetails = ({
		id,
		albumId,
	}: {
		id: number;
		albumId?: number;
	}): Promise<SongDetailsContract> => {
		return this.httpClient.get<SongDetailsContract>(
			this.urlMapper.mapRelative(`/api/songs/${id}/details`),
			{ albumId: albumId },
		);
	};

	getSongWithArchivedVersions = ({
		id,
	}: {
		id: number;
	}): Promise<EntryWithArchivedVersionsContract<SongApiContract>> => {
		return this.httpClient.get<
			EntryWithArchivedVersionsContract<SongApiContract>
		>(this.urlMapper.mapRelative(`/api/songs/${id}/versions`));
	};

	getVersionDetails = ({
		id,
		comparedVersionId,
	}: {
		id: number;
		comparedVersionId?: number;
	}): Promise<ArchivedSongVersionDetailsContract> => {
		return this.httpClient.get<ArchivedSongVersionDetailsContract>(
			this.urlMapper.mapRelative(`/api/songs/versions/${id}`),
			{ comparedVersionId: comparedVersionId },
		);
	};

	create = (
		requestToken: string,
		contract: CreateSongContract,
	): Promise<number> => {
		const formData = new FormData();
		formData.append('contract', JSON.stringify(contract));

		return this.httpClient.post<number>(
			this.urlMapper.mapRelative(`/api/songs`),
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
		contract: SongForEditContract,
	): Promise<number> => {
		const formData = new FormData();
		formData.append('contract', JSON.stringify(contract));

		return this.httpClient.post<number>(
			this.urlMapper.mapRelative(`/api/songs/${contract.id}`),
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
		{ id, targetSongId }: { id: number; targetSongId: number },
	): Promise<void> => {
		return this.httpClient.post(
			this.urlMapper.mapRelative(
				`/api/songs/${id}/merge?${qs.stringify({
					targetSongId: targetSongId,
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

	pvPlayerWithRating = ({
		songId,
	}: {
		songId: number;
	}): Promise<SongWithPVPlayerAndVoteContract> => {
		return this.httpClient.get<SongWithPVPlayerAndVoteContract>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/with-rating`),
		);
	};

	delete = (
		requestToken: string,
		{ id, notes }: { id: number; notes: string },
	): Promise<void> => {
		return this.httpClient.delete(
			this.urlMapper.mapRelative(
				`/api/songs/${id}?${qs.stringify({
					notes: notes,
				})}`,
			),
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	updateVersionVisibility = (
		requestToken: string,
		{
			archivedVersionId,
			hidden,
		}: {
			archivedVersionId: number;
			hidden: boolean;
		},
	): Promise<void> => {
		return this.httpClient.post(
			this.urlMapper.mapRelative(
				`/api/songs/versions/${archivedVersionId}/update-visibility?${qs.stringify(
					{
						hidden: hidden,
					},
				)}`,
			),
			undefined,
			{ headers: { requestVerificationToken: requestToken } },
		);
	};

	revertToVersion = (
		requestToken: string,
		{
			archivedVersionId,
		}: {
			archivedVersionId: number;
		},
	): Promise<number> => {
		return this.httpClient.post<number>(
			this.urlMapper.mapRelative(
				`/api/songs/versions/${archivedVersionId}/revert`,
			),
			undefined,
			{ headers: { requestVerificationToken: requestToken } },
		);
	};
}

export interface PVEmbedParams {
	enableScriptAccess?: boolean;

	elementId?: string;

	pvServices?: string;
}

export interface SongQueryParams extends CommonQueryParams {
	sort?: string;

	songTypes?: SongType[];
}

export const songRepo = new SongRepository(httpClient, vdbConfig.baseAddress);
