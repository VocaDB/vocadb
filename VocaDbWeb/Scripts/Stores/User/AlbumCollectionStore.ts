import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import {
	AlbumForUserForApiContract,
	MediaType,
} from '@/DataContracts/User/AlbumForUserForApiContract';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { EntryType } from '@/Models/EntryType';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { GlobalValues } from '@/Shared/GlobalValues';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';
import { AdvancedSearchFilters } from '@/Stores/Search/AdvancedSearchFilters';
import { AlbumSortRule } from '@/Stores/Search/AlbumSearchStore';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import {
	includesAny,
	StateChangeEvent,
	LocationStateStore,
} from '@/route-sphere';
import Ajv from 'ajv';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

import schema from './AlbumCollectionRouteParams.schema.json';

interface AlbumCollectionRouteParams {
	advancedFilters?: AdvancedSearchFilter[];
	artistId?: number;
	collectionStatus?: string;
	discType?: AlbumType;
	eventId?: number;
	filter?: string;
	page?: number;
	pageSize?: number;
	sort?: AlbumSortRule;
	tagId?: number;
	viewMode?: 'Details' | 'Tiles' /* TODO: enum */;
	mediaType?: MediaType;
}

const clearResultsByQueryKeys: (keyof AlbumCollectionRouteParams)[] = [
	'pageSize',
	'filter',
	'tagId',

	'advancedFilters',
	'discType',
	'artistId',
	'collectionStatus',
	'eventId',
	'mediaType',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<AlbumCollectionRouteParams>(schema);

export class AlbumCollectionStore
	implements LocationStateStore<AlbumCollectionRouteParams> {
	readonly advancedFilters = new AdvancedSearchFilters();
	@observable albumType = AlbumType.Unknown;
	readonly artist: BasicEntryLinkStore<ArtistContract>;
	@observable artistName = '';
	@observable collectionStatus = '';
	@observable loading = true; // Currently loading for data
	@observable page: AlbumForUserForApiContract[] = []; // Current page of items
	readonly paging = new ServerSidePagingStore(20); // Paging store
	pauseNotifications = false;
	readonly releaseEvent: BasicEntryLinkStore<ReleaseEventContract>;
	@observable searchTerm = '';
	@observable sort = AlbumSortRule.Name;
	readonly tag: BasicEntryLinkStore<TagBaseContract>;
	@observable viewMode: 'Details' | 'Tiles' = 'Details'; /* TODO: enum */
	@observable mediaType?: MediaType;

	constructor(
		private readonly values: GlobalValues,
		private readonly userRepo: UserRepository,
		artistRepo: ArtistRepository,
		eventRepo: ReleaseEventRepository,
		tagRepo: TagRepository,
		private readonly userId: number,
		readonly publicCollection: boolean,
	) {
		makeObservable(this);

		this.artist = new BasicEntryLinkStore<ArtistContract>((entryId) =>
			artistRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);

		this.releaseEvent = new BasicEntryLinkStore<ReleaseEventContract>(
			(entryId) =>
				eventRepo
					? eventRepo.getOne({ id: entryId })
					: Promise.resolve(undefined),
		);

		this.tag = new BasicEntryLinkStore<TagBaseContract>((entryId) =>
			tagRepo.getById({ id: entryId, lang: values.languagePreference }),
		);
	}

	@computed get tagId(): number | undefined {
		return this.tag.id;
	}

	@computed get tagName(): string | undefined {
		return this.tag.name;
	}

	@computed get tagUrl(): string | undefined {
		return EntryUrlMapper.details_tag_contract(this.tag.entry);
	}

	@computed get releaseEventUrl(): string {
		return EntryUrlMapper.details(
			EntryType.ReleaseEvent,
			this.releaseEvent.id!,
		);
	}

	ratingStars = (userRating: number): { enabled: boolean }[] => {
		const ratings = [1, 2, 3, 4, 5].map((rating) => ({
			enabled: Math.round(userRating) >= rating,
		}));
		return ratings;
	};

	@action updateResults = async (
		clearResults: boolean = true,
	): Promise<void> => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;
		this.loading = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);

		const result = await this.userRepo.getAlbumCollectionList({
			userId: this.userId,
			paging: pagingProperties,
			lang: this.values.languagePreference,
			query: this.searchTerm,
			tag: this.tagId,
			albumType: this.albumType,
			artistId: this.artist.id,
			purchaseStatuses: this.collectionStatus,
			releaseEventId: this.releaseEvent.id,
			advancedFilters: this.advancedFilters.filters,
			sort: this.sort,
			mediaType: this.mediaType,
		});

		this.pauseNotifications = false;

		runInAction(() => {
			if (pagingProperties.getTotalCount)
				this.paging.totalItems = result.totalCount;

			this.page = result.items;
			this.loading = false;
		});
	};

	@computed.struct get locationState(): AlbumCollectionRouteParams {
		return {
			advancedFilters: this.advancedFilters.filters.map((filter) => ({
				description: filter.description,
				filterType: filter.filterType,
				negate: filter.negate,
				param: filter.param,
			})),
			artistId: this.artist.id,
			collectionStatus: this.collectionStatus,
			discType: this.albumType,
			eventId: this.releaseEvent.id,
			filter: this.searchTerm,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			sort: this.sort,
			tagId: this.tag.id,
			viewMode: this.viewMode,
			mediaType: this.mediaType,
		};
	}
	set locationState(value: AlbumCollectionRouteParams) {
		this.advancedFilters.filters = value.advancedFilters ?? [];
		this.artist.id = value.artistId;
		this.collectionStatus = value.collectionStatus ?? '';
		this.albumType = value.discType ?? AlbumType.Unknown;
		this.releaseEvent.id = value.eventId;
		this.searchTerm = value.filter ?? '';
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 20;
		this.sort = value.sort ?? AlbumSortRule.Name;
		this.tag.id = value.tagId;
		this.viewMode = value.viewMode || 'Details';
		this.mediaType = value.mediaType;
	}

	validateLocationState = (data: any): data is AlbumCollectionRouteParams => {
		return validate(data);
	};

	onLocationStateChange = (
		event: StateChangeEvent<AlbumCollectionRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
