import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { Tag } from '@/Models/Tags/Tag';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { EntryRepository } from '@/Repositories/EntryRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import { PVPlayersFactory } from '@/Stores/PVs/PVPlayersFactory';
import {
	AlbumSearchRouteParams,
	AlbumSearchStore,
} from '@/Stores/Search/AlbumSearchStore';
import {
	AnythingSearchRouteParams,
	AnythingSearchStore,
} from '@/Stores/Search/AnythingSearchStore';
import {
	ArtistSearchRouteParams,
	ArtistSearchStore,
} from '@/Stores/Search/ArtistSearchStore';
import { ICommonSearchStore } from '@/Stores/Search/CommonSearchStore';
import {
	EventSearchRouteParams,
	EventSearchStore,
} from '@/Stores/Search/EventSearchStore';
import { ISearchCategoryBaseStore } from '@/Stores/Search/SearchCategoryBaseStore';
import {
	SongSearchRouteParams,
	SongSearchStore,
} from '@/Stores/Search/SongSearchStore';
import { TagFilters } from '@/Stores/Search/TagFilters';
import {
	TagSearchRouteParams,
	TagSearchStore,
} from '@/Stores/Search/TagSearchStore';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import { StateChangeEvent, LocationStateStore } from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import addFormats from 'ajv-formats';
import {
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export enum SearchType {
	Anything = 'Anything',
	Artist = 'Artist',
	Album = 'Album',
	ReleaseEvent = 'ReleaseEvent',
	Song = 'Song',
	Tag = 'Tag',
}

export type SearchRouteParams =
	| AnythingSearchRouteParams
	| AlbumSearchRouteParams
	| ArtistSearchRouteParams
	| EventSearchRouteParams
	| SongSearchRouteParams
	| TagSearchRouteParams;

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });
addFormats(ajv);

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<SearchRouteParams> = require('./SearchRouteParams.schema');
const validate = ajv.compile(schema);

export class SearchStore
	implements ICommonSearchStore, LocationStateStore<SearchRouteParams> {
	readonly albumSearchStore: AlbumSearchStore;
	readonly anythingSearchStore: AnythingSearchStore;
	readonly artistSearchStore: ArtistSearchStore;
	readonly eventSearchStore: EventSearchStore;
	readonly songSearchStore: SongSearchStore;
	readonly tagSearchStore: TagSearchStore;

	@observable draftsOnly = false;
	@observable genreTags: TagBaseContract[] = [];
	@observable pageSize = 10;
	@observable showAdvancedFilters = false;
	@observable searchTerm = '';
	@observable searchType = SearchType.Anything;
	readonly tagFilters: TagFilters;
	@observable showTags = false;

	constructor(
		values: GlobalValues,
		urlMapper: UrlMapper,
		entryRepo: EntryRepository,
		artistRepo: ArtistRepository,
		albumRepo: AlbumRepository,
		songRepo: SongRepository,
		eventRepo: ReleaseEventRepository,
		tagRepo: TagRepository,
		userRepo: UserRepository,
		pvPlayersFactory: PVPlayersFactory,
	) {
		makeObservable(this);

		this.tagFilters = new TagFilters(values, tagRepo);

		this.anythingSearchStore = new AnythingSearchStore(this, values, entryRepo);
		this.artistSearchStore = new ArtistSearchStore(this, values, artistRepo);
		this.albumSearchStore = new AlbumSearchStore(
			this,
			values,
			albumRepo,
			artistRepo,
		);
		this.eventSearchStore = new EventSearchStore(
			this,
			values,
			eventRepo,
			artistRepo,
		);
		this.songSearchStore = new SongSearchStore(
			this,
			values,
			urlMapper,
			songRepo,
			userRepo,
			eventRepo,
			artistRepo,
			pvPlayersFactory,
		);
		this.tagSearchStore = new TagSearchStore(this, values, tagRepo);

		reaction(() => this.showTags, this.updateResultsWithTotalCount);

		tagRepo
			.getTopTags({
				lang: values.languagePreference,
				categoryName: Tag.commonCategory_Genres,
				entryType: undefined,
			})
			.then((result) =>
				runInAction(() => {
					this.genreTags = result;
				}),
			);
	}

	@computed get showAnythingSearch(): boolean {
		return this.searchType === SearchType.Anything;
	}

	@computed get showArtistSearch(): boolean {
		return this.searchType === SearchType.Artist;
	}

	@computed get showAlbumSearch(): boolean {
		return this.searchType === SearchType.Album;
	}

	@computed get showEventSearch(): boolean {
		return this.searchType === SearchType.ReleaseEvent;
	}

	@computed get showSongSearch(): boolean {
		return this.searchType === SearchType.Song;
	}

	@computed get showTagSearch(): boolean {
		return this.searchType === SearchType.Tag;
	}

	@computed get showTagFilter(): boolean {
		return !this.showTagSearch;
	}

	@computed get showDraftsFilter(): boolean {
		return this.searchType !== SearchType.Tag;
	}

	@computed get isUniversalSearch(): boolean {
		return this.searchType === SearchType.Anything;
	}

	private getCategoryStore = (
		searchType: SearchType,
	): ISearchCategoryBaseStore<SearchRouteParams> => {
		switch (searchType) {
			case SearchType.Anything:
				return this
					.anythingSearchStore as ISearchCategoryBaseStore<SearchRouteParams>;
			case SearchType.Artist:
				return this
					.artistSearchStore as ISearchCategoryBaseStore<SearchRouteParams>;
			case SearchType.Album:
				return this
					.albumSearchStore as ISearchCategoryBaseStore<SearchRouteParams>;
			case SearchType.ReleaseEvent:
				return this
					.eventSearchStore as ISearchCategoryBaseStore<SearchRouteParams>;
			case SearchType.Song:
				return this
					.songSearchStore as ISearchCategoryBaseStore<SearchRouteParams>;
			case SearchType.Tag:
				return this
					.tagSearchStore as ISearchCategoryBaseStore<SearchRouteParams>;
		}
	};

	get currentCategoryStore(): ISearchCategoryBaseStore<SearchRouteParams> {
		return this.getCategoryStore(this.searchType);
	}

	get paging(): ServerSidePagingStore {
		return this.currentCategoryStore.paging;
	}

	@computed get locationState(): SearchRouteParams {
		return this.currentCategoryStore.locationState;
	}
	set locationState(value: SearchRouteParams) {
		value.searchType ??= SearchType.Anything;
		this.searchType = value.searchType;
		this.currentCategoryStore.locationState = value;
	}

	validateLocationState = (data: any): data is SearchRouteParams => {
		return validate(data);
	};

	updateResults = (clearResults: boolean): Promise<void> => {
		return this.currentCategoryStore.updateResults(clearResults);
	};

	updateResultsWithTotalCount = (): Promise<void> => {
		return this.currentCategoryStore.updateResultsWithTotalCount();
	};

	onLocationStateChange = (
		event: StateChangeEvent<SearchRouteParams>,
	): void => {
		this.currentCategoryStore.onLocationStateChange?.(event);
	};
}
