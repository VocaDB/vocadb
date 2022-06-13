import { ISongSearchStore } from '@Components/Search/Partials/SongSearchList';
import SongListBaseContract from '@DataContracts/SongListBaseContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import PVServiceIcons from '@Models/PVServiceIcons';
import ArtistRepository from '@Repositories/ArtistRepository';
import SongRepository from '@Repositories/SongRepository';
import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import GlobalValues from '@Shared/GlobalValues';
import UrlMapper from '@Shared/UrlMapper';
import AdvancedSearchFilters from '@Stores/Search/AdvancedSearchFilters';
import ArtistFilters from '@Stores/Search/ArtistFilters';
import TagFilters from '@Stores/Search/TagFilters';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
import SongWithPreviewStore from '@Stores/Song/SongWithPreviewStore';
import { SongListSortRule } from '@Stores/SongList/SongListsBaseStore';
import { StoreWithPagination } from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';
import moment from 'moment';

import PVPlayerStore from '../PVs/PVPlayerStore';
import PVPlayersFactory from '../PVs/PVPlayersFactory';
import AdvancedSearchFilter from '../Search/AdvancedSearchFilter';
import {
	IRatedSongSearchItem,
	SongVoteRating,
} from '../Search/SongSearchStore';
import TagFilter from '../Search/TagFilter';
import PlayListRepositoryForRatedSongsAdapter, {
	IRatedSongsAdapterStore,
} from '../Song/PlayList/PlayListRepositoryForRatedSongsAdapter';
import PlayListStore from '../Song/PlayList/PlayListStore';

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

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<RatedSongsSearchRouteParams> = require('./RatedSongsSearchRouteParams.schema');
const validate = ajv.compile(schema);

export default class RatedSongsSearchStore
	implements
		StoreWithPagination<RatedSongsSearchRouteParams>,
		ISongSearchStore,
		IRatedSongsAdapterStore {
	public readonly advancedFilters = new AdvancedSearchFilters();
	public artistFilters: ArtistFilters;
	@observable public groupByRating = true;
	public isInit = false;
	@observable public loading = true; // Currently loading for data
	@observable public page: IRatedSongSearchItem[] = []; // Current page of items
	public readonly paging = new ServerSidePagingStore(20); // Paging view model
	public readonly playListStore: PlayListStore;
	public readonly pvPlayerStore: PVPlayerStore;
	public pvServiceIcons: PVServiceIcons;
	@observable public rating: SongVoteRating = 'Nothing' /* TODO: enum */;
	@observable public searchTerm = '';
	@observable public showTags = false;
	@observable public songListId?: number;
	@observable public songLists: SongListBaseContract[] = [];
	@observable public sort = RatedSongForUserSortRule.RatingDate;
	public readonly tagFilters: TagFilters;
	@observable public viewMode: 'Details' | 'PlayList' =
		'Details' /* TODO: enum */;

	public constructor(
		private readonly values: GlobalValues,
		urlMapper: UrlMapper,
		private readonly userRepo: UserRepository,
		artistRepo: ArtistRepository,
		private readonly songRepo: SongRepository,
		tagRepo: TagRepository,
		public readonly userId: number,
		pvPlayersFactory: PVPlayersFactory,
		initialize = true,
	) {
		makeObservable(this);

		this.artistFilters = new ArtistFilters(values, artistRepo);
		this.pvServiceIcons = new PVServiceIcons(urlMapper);
		this.tagFilters = new TagFilters(values, tagRepo);

		reaction(() => this.showTags, this.updateResultsWithoutTotalCount);

		this.pvPlayerStore = new PVPlayerStore(
			values,
			songRepo,
			userRepo,
			pvPlayersFactory,
		);

		const songsRepoAdapter = new PlayListRepositoryForRatedSongsAdapter(
			userRepo,
			this,
		);

		this.playListStore = new PlayListStore(
			values,
			urlMapper,
			songsRepoAdapter,
			this.pvPlayerStore,
		);

		if (initialize) this.init();
	}

	@computed public get tagIds(): number[] {
		return this.tagFilters.tags.map((t) => t.id);
	}
	public set tagIds(value: number[]) {
		// OPTIMIZE
		this.tagFilters.tags = [];
		this.tagFilters.addTags(value);
	}

	@computed public get fields(): string {
		return `AdditionalNames,MainPicture${
			this.showTags ? ',Tags' : ''
		}` /* TODO: enum */;
	}

	@action public selectTag = (tag: TagBaseContract): void => {
		this.tagFilters.tags = [TagFilter.fromContract(tag)];
	};

	public init = (): void => {
		if (this.isInit) return;

		this.userRepo
			.getSongLists({
				userId: this.userId,
				query: undefined,
				paging: { start: 0, maxEntries: 50, getTotalCount: false },
				tagIds: [],
				sort: SongListSortRule.Name,
				fields: undefined,
			})
			.then((songLists) =>
				runInAction(() => {
					this.songLists = songLists.items;
				}),
			);

		this.isInit = true;
	};

	public formatDate = (dateStr: string): string => {
		return moment(dateStr).format('l');
	};

	public getPVServiceIcons = (
		services: string,
	): { service: string; url: string }[] => {
		return this.pvServiceIcons.getIconUrls(services);
	};

	public pauseNotifications = false;

	@action public updateResults = async (
		clearResults: boolean = true,
	): Promise<void> => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;
		this.loading = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);

		if (this.viewMode === 'PlayList') {
			this.playListStore.updateResultsWithTotalCount().then(() => {
				this.pauseNotifications = false;
				runInAction(() => {
					this.loading = false;
				});
			});
			return;
		}

		const result = await this.userRepo.getRatedSongsList({
			userId: this.userId,
			paging: pagingProperties,
			lang: this.values.languagePreference,
			query: this.searchTerm,
			tagIds: this.tagFilters.tagIds,
			artistIds: this.artistFilters.artistIds,
			childVoicebanks: this.artistFilters.childVoicebanks,
			rating: this.rating,
			songListId: this.songListId,
			advancedFilters: this.advancedFilters.filters,
			groupByRating: this.groupByRating,
			pvServices: undefined,
			fields: this.fields,
			sort: this.sort,
		});

		var songs: IRatedSongSearchItem[] = [];

		_.each(result.items, (item) => {
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
		});

		this.pauseNotifications = false;

		runInAction(() => {
			if (pagingProperties.getTotalCount)
				this.paging.totalItems = result.totalCount;

			this.page = songs;
			this.loading = false;
		});
	};

	public updateResultsWithTotalCount = (): Promise<void> => {
		return this.updateResults(true);
	};

	public updateResultsWithoutTotalCount = (): Promise<void> => {
		return this.updateResults(false);
	};

	public popState = false;

	public readonly clearResultsByQueryKeys: (keyof RatedSongsSearchRouteParams)[] = [
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

	@computed.struct public get routeParams(): RatedSongsSearchRouteParams {
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
	public set routeParams(value: RatedSongsSearchRouteParams) {
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

	public validateRouteParams = (
		data: any,
	): data is RatedSongsSearchRouteParams => {
		return validate(data);
	};

	public onClearResults = (): void => {
		this.paging.goToFirstPage();
	};
}
