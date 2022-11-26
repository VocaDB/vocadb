import { SongListBaseContract } from '@/DataContracts/SongListBaseContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { PVServiceIcons } from '@/Models/PVServiceIcons';
import { ISongSearchStore } from '@/Pages/Search/Partials/SongSearchList';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import {
	SongOptionalField,
	SongRepository,
} from '@/Repositories/SongRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import type { UserGetRatedSongsListQueryParams } from '@/Repositories/UserRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';
import { AdvancedSearchFilters } from '@/Stores/Search/AdvancedSearchFilters';
import { ArtistFilters } from '@/Stores/Search/ArtistFilters';
import { IRatedSongSearchItem } from '@/Stores/Search/SongSearchStore';
import type { SongVoteRating } from '@/Stores/Search/SongSearchStore';
import { TagFilter } from '@/Stores/Search/TagFilter';
import { TagFilters } from '@/Stores/Search/TagFilters';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import { SongWithPreviewStore } from '@/Stores/Song/SongWithPreviewStore';
import { SongListSortRule } from '@/Stores/SongList/SongListsBaseStore';
import {
	includesAny,
	StateChangeEvent,
	LocationStateStore,
} from '@vocadb/route-sphere';
import Ajv from 'ajv';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';
import moment from 'moment';

import schema from './RatedSongsSearchRouteParams.schema.json';

export enum RatedSongForUserSortRule {
	None = 'None',
	Name = 'Name',
	AdditionDate = 'AdditionDate',
	PublishDate = 'PublishDate',
	FavoritedTimes = 'FavoritedTimes',
	RatingScore = 'RatingScore',
	RatingDate = 'RatingDate',
}

interface RatedSongsSearchRouteParams {
	advancedFilters?: AdvancedSearchFilter[];
	artistId?: number | number[];
	artistParticipationStatus?: string /* TODO: enum */;
	childVoicebanks?: boolean;
	filter?: string;
	groupByRating?: boolean;
	includeMembers?: boolean;
	page?: number;
	pageSize?: number;
	rating?: SongVoteRating;
	songListId?: number;
	sort?: RatedSongForUserSortRule;
	tagId?: number | number[];
	viewMode?: 'Details' | 'PlayList' /* TODO: enum */;
}

const clearResultsByQueryKeys: (keyof RatedSongsSearchRouteParams)[] = [
	'pageSize',
	'filter',
	'tagId',

	'advancedFilters',
	'artistId',
	'artistParticipationStatus',
	'childVoicebanks',
	'includeMembers',
	'rating',
	'songListId',
	'sort',
	'viewMode',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<RatedSongsSearchRouteParams>(schema);

export class RatedSongsSearchStore
	implements LocationStateStore<RatedSongsSearchRouteParams>, ISongSearchStore {
	readonly advancedFilters = new AdvancedSearchFilters();
	artistFilters: ArtistFilters;
	@observable groupByRating = true;
	isInit = false;
	@observable loading = true; // Currently loading for data
	@observable page: IRatedSongSearchItem[] = []; // Current page of items
	readonly paging = new ServerSidePagingStore(20); // Paging view model
	pvServiceIcons: PVServiceIcons;
	@observable rating: SongVoteRating = 'Nothing' /* TODO: enum */;
	@observable searchTerm = '';
	@observable showTags = false;
	@observable songListId?: number;
	@observable songLists: SongListBaseContract[] = [];
	@observable sort = RatedSongForUserSortRule.RatingDate;
	readonly tagFilters: TagFilters;
	@observable viewMode: 'Details' | 'PlayList' = 'Details' /* TODO: enum */;

	constructor(
		private readonly values: GlobalValues,
		urlMapper: UrlMapper,
		private readonly userRepo: UserRepository,
		artistRepo: ArtistRepository,
		private readonly songRepo: SongRepository,
		tagRepo: TagRepository,
		readonly userId: number,
		initialize = true,
	) {
		makeObservable(this);

		this.artistFilters = new ArtistFilters(values, artistRepo);
		this.pvServiceIcons = new PVServiceIcons(urlMapper);
		this.tagFilters = new TagFilters(values, tagRepo);

		reaction(() => this.showTags, this.updateResultsWithoutTotalCount);

		if (initialize) this.init();
	}

	@computed get tagIds(): number[] {
		return this.tagFilters.tags.map((t) => t.id);
	}
	set tagIds(value: number[]) {
		// OPTIMIZE
		this.tagFilters.tags = [];
		this.tagFilters.addTags(value);
	}

	@computed get fields(): SongOptionalField[] {
		return this.showTags
			? [
					SongOptionalField.AdditionalNames,
					SongOptionalField.MainPicture,
					SongOptionalField.Tags,
			  ]
			: [SongOptionalField.AdditionalNames, SongOptionalField.MainPicture];
	}

	@action selectTag = (tag: TagBaseContract): void => {
		this.tagFilters.tags = [TagFilter.fromContract(tag)];
	};

	init = async (): Promise<void> => {
		if (this.isInit) return;

		const songLists = await this.userRepo.getSongLists({
			userId: this.userId,
			query: undefined,
			paging: { start: 0, maxEntries: 50, getTotalCount: false },
			tagIds: [],
			sort: SongListSortRule.Name,
			fields: undefined,
		});

		runInAction(() => {
			this.songLists = songLists.items;
		});

		this.isInit = true;
	};

	formatDate = (dateStr: string): string => {
		return moment(dateStr).format('l');
	};

	getPVServiceIcons = (
		services: string,
	): { service: string; url: string }[] => {
		return this.pvServiceIcons.getIconUrls(services);
	};

	@computed get queryParams(): UserGetRatedSongsListQueryParams {
		return {
			userId: this.userId,
			query: this.searchTerm,
			tagIds: this.tagFilters.tagIds,
			artistIds: this.artistFilters.artistIds,
			childVoicebanks: this.artistFilters.childVoicebanks,
			rating: this.rating,
			songListId: this.songListId,
			advancedFilters: this.advancedFilters.filters,
			groupByRating: this.groupByRating,
			sort: this.sort,
		};
	}

	pauseNotifications = false;

	@action updateResults = async (
		clearResults: boolean = true,
	): Promise<void> => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;
		this.loading = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);

		if (this.viewMode === 'PlayList') {
			this.pauseNotifications = false;
			runInAction(() => {
				this.loading = false;
			});
			return;
		}

		const result = await this.userRepo.getRatedSongsList({
			fields: this.fields,
			lang: this.values.languagePreference,
			paging: pagingProperties,
			pvServices: undefined,
			queryParams: this.queryParams,
		});

		var songs: IRatedSongSearchItem[] = [];

		for (const item of result.items) {
			const song: IRatedSongSearchItem = item.song!;

			song.rating = item.rating;

			if (song.pvServices && song.pvServices !== 'Nothing') {
				song.previewStore = new SongWithPreviewStore(
					this.songRepo,
					this.userRepo,
					song.id,
					song.pvServices,
				);
				// TODO: song.previewStore.ratingComplete =
			} else {
				song.previewStore = undefined;
			}

			songs.push(song);
		}

		this.pauseNotifications = false;

		runInAction(() => {
			if (pagingProperties.getTotalCount)
				this.paging.totalItems = result.totalCount;

			this.page = songs;
			this.loading = false;
		});
	};

	updateResultsWithTotalCount = (): Promise<void> => {
		return this.updateResults(true);
	};

	updateResultsWithoutTotalCount = (): Promise<void> => {
		return this.updateResults(false);
	};

	@computed.struct get locationState(): RatedSongsSearchRouteParams {
		return {
			advancedFilters: this.advancedFilters.filters.map((filter) => ({
				description: filter.description,
				filterType: filter.filterType,
				negate: filter.negate,
				param: filter.param,
			})),
			artistId: this.artistFilters.artistIds,
			artistParticipationStatus: this.artistFilters.artistParticipationStatus,
			// TODO: autoplay
			childVoicebanks: this.artistFilters.childVoicebanks,
			filter: this.searchTerm,
			groupByRating: this.groupByRating,
			// TODO: includeMembers
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			rating: this.rating,
			// TODO: shuffle
			songListId: this.songListId,
			sort: this.sort,
			tagId: this.tagIds,
			viewMode: this.viewMode,
		};
	}
	set locationState(value: RatedSongsSearchRouteParams) {
		this.advancedFilters.filters = value.advancedFilters ?? [];
		this.artistFilters.artistIds = ([] as number[]).concat(
			value.artistId ?? [],
		);
		this.artistFilters.artistParticipationStatus =
			value.artistParticipationStatus ?? 'Everything';
		// TODO: autoplay
		this.artistFilters.childVoicebanks = value.childVoicebanks ?? false;
		this.searchTerm = value.filter ?? '';
		this.groupByRating = value.groupByRating ?? true;
		// TODO: includeMembers
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 10;
		this.rating = value.rating ?? 'Nothing';
		// TODO: shuffle
		this.songListId = value.songListId;
		this.sort = value.sort ?? RatedSongForUserSortRule.RatingDate;
		this.tagIds = ([] as number[]).concat(value.tagId ?? []);
		this.viewMode = value.viewMode ?? 'Details';
	}

	validateLocationState = (data: any): data is RatedSongsSearchRouteParams => {
		return validate(data);
	};

	onLocationStateChange = (
		event: StateChangeEvent<RatedSongsSearchRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
