import CountPerDayContract from '@DataContracts/Aggregate/CountPerDayContract';
import ArtistContract from '@DataContracts/Artist/ArtistContract';
import CommentContract from '@DataContracts/CommentContract';
import NewSongCheckResultContract from '@DataContracts/NewSongCheckResultContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import LyricsForSongContract from '@DataContracts/Song/LyricsForSongContract';
import SongApiContract from '@DataContracts/Song/SongApiContract';
import SongContract from '@DataContracts/Song/SongContract';
import SongForEditContract from '@DataContracts/Song/SongForEditContract';
import SongWithPVPlayerAndVoteContract from '@DataContracts/Song/SongWithPVPlayerAndVoteContract';
import SongListBaseContract from '@DataContracts/SongListBaseContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import RatedSongForUserForApiContract from '@DataContracts/User/RatedSongForUserForApiContract';
import AjaxHelper from '@Helpers/AjaxHelper';
import TimeUnit from '@Models/Aggregate/TimeUnit';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import PVService from '@Models/PVs/PVService';
import SongVoteRating from '@Models/SongVoteRating';
import SongType from '@Models/Songs/SongType';
import functions from '@Shared/GlobalFunctions';
import HttpClient, { HeaderNames, MediaTypes } from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import AdvancedSearchFilter from '@ViewModels/Search/AdvancedSearchFilter';

import BaseRepository from './BaseRepository';
import { CommonQueryParams } from './BaseRepository';
import ICommentRepository from './ICommentRepository';

// Repository for managing songs and related objects.
// Corresponds to the SongController class.
export default class SongRepository
	extends BaseRepository
	implements ICommentRepository {
	private readonly urlMapper: UrlMapper;

	public constructor(private readonly httpClient: HttpClient, baseUrl: string) {
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

		this.pvPlayerWithRating = ({
			songId,
		}: {
			songId: number;
		}): Promise<SongWithPVPlayerAndVoteContract> => {
			return this.getJSON<SongWithPVPlayerAndVoteContract>(
				'/PVPlayerWithRating',
				{ songId: songId },
			);
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

	public addSongToList = ({
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

	public createComment = ({
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

	public createReport = ({
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

	public deleteComment = ({
		commentId,
	}: {
		commentId: number;
	}): Promise<void> => {
		return this.httpClient.delete<void>(
			this.urlMapper.mapRelative(`/api/songs/comments/${commentId}`),
		);
	};

	public findDuplicate = ({
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

	public getByNames({
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
			lang: ContentLanguagePreference[lang],
			ignoreIds: ignoreIds,
		});
	}

	public getComments = ({
		entryId: songId,
	}: {
		entryId: number;
	}): Promise<CommentContract[]> => {
		return this.httpClient.get<CommentContract[]>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/comments`),
		);
	};

	public getForEdit = ({
		id,
	}: {
		id: number;
	}): Promise<SongForEditContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/songs/${id}/for-edit`);
		return this.httpClient.get<SongForEditContract>(url);
	};

	public getLyrics = ({
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

	public getOneWithComponents = ({
		id,
		fields,
		lang,
	}: {
		id: number;
		fields: string;
		lang: ContentLanguagePreference;
	}): Promise<SongApiContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/songs/${id}`);
		return this.httpClient.get<SongApiContract>(url, {
			fields: fields,
			lang: ContentLanguagePreference[lang],
		});
	};

	public getOne = ({
		id,
		lang,
	}: {
		id: number;
		lang: ContentLanguagePreference;
	}): Promise<SongContract> => {
		var url = functions.mergeUrls(this.baseUrl, `/api/songs/${id}`);
		return this.httpClient.get<SongContract>(url, {
			fields: 'AdditionalNames',
			lang: ContentLanguagePreference[lang],
		});
	};

	public getListByParams({
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

	public getList = ({
		paging,
		lang,
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
		pvServices,
		since,
		minScore,
		userCollectionId,
		parentSongId,
		fields,
		status,
		advancedFilters,
		minMilliBpm,
		maxMilliBpm,
		minLength,
		maxLength,
	}: {
		paging: PagingProperties;
		lang: ContentLanguagePreference;
		query: string;
		sort: string;
		songTypes?: string;
		afterDate: Date;
		beforeDate: Date;
		tagIds: number[];
		childTags: boolean;
		unifyTypesAndTags: boolean;
		artistIds: number[];
		artistParticipationStatus: string;
		childVoicebanks: boolean;
		includeMembers: boolean;
		eventId: number;
		onlyWithPvs: boolean;
		pvServices?: string;
		since: number;
		minScore: number;
		userCollectionId?: number;
		parentSongId: number;
		fields: string;
		status?: string;
		advancedFilters?: AdvancedSearchFilter[];
		minMilliBpm?: number;
		maxMilliBpm?: number;
		minLength?: number;
		maxLength?: number;
	}): Promise<PartialFindResultContract<SongContract>> => {
		var url = functions.mergeUrls(this.baseUrl, '/api/songs');
		var data = {
			start: paging.start,
			getTotalCount: paging.getTotalCount,
			maxResults: paging.maxEntries,
			query: query,
			fields: fields,
			lang: ContentLanguagePreference[lang],
			nameMatchMode: 'Auto',
			sort: sort,
			songTypes: songTypes,
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
			pvServices: pvServices,
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

	public getOverTime = ({
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
	public getPvId = ({
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

	public getRatings = ({
		songId,
	}: {
		songId: number;
	}): Promise<RatedSongForUserForApiContract[]> => {
		return this.httpClient.get<RatedSongForUserForApiContract[]>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/ratings`),
			{ userFields: 'MainPicture' },
		);
	};

	public getTagSuggestions = ({
		songId,
	}: {
		songId: number;
	}): Promise<TagUsageForApiContract[]> => {
		return this.httpClient.get<TagUsageForApiContract[]>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/tagSuggestions`),
		);
	};

	// Maps a relative URL to an absolute one.
	private mapUrl: (relative: string) => string;

	private post: <T>(relative: string, params: any) => Promise<T>;

	public pvForSongAndService: ({
		songId,
		pvService,
	}: {
		songId: number;
		pvService: PVService;
	}) => Promise<string>;

	public pvPlayer = ({
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

	public pvPlayerWithRating: ({
		songId,
	}: {
		songId: number;
	}) => Promise<SongWithPVPlayerAndVoteContract>;

	//public songListsForSong: (songId: number, callback: (result: SongListContract[]) => void) => void;

	public songListsForSong: ({ songId }: { songId: number }) => Promise<string>;

	public songListsForUser: ({
		ignoreSongId,
	}: {
		ignoreSongId: number;
	}) => Promise<SongListBaseContract[]>;

	public updateComment = ({
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

	public updatePersonalDescription = ({
		songId,
		text,
		author,
	}: {
		songId: number;
		text: string;
		author: ArtistContract;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(`/api/songs/${songId}/personal-description/`),
			{
				personalDescriptionText: text,
				personalDescriptionAuthor: author || undefined,
			},
		);
	};

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
}

export interface PVEmbedParams {
	enableScriptAccess?: boolean;

	elementId?: string;

	pvServices?: string;
}

export interface SongQueryParams extends CommonQueryParams {
	sort?: string;

	songTypes?: string;
}
