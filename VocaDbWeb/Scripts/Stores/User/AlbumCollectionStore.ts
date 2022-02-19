import ArtistContract from '@DataContracts/Artist/ArtistContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import AlbumForUserForApiContract from '@DataContracts/User/AlbumForUserForApiContract';
import AlbumType from '@Models/Albums/AlbumType';
import EntryType from '@Models/EntryType';
import ArtistRepository from '@Repositories/ArtistRepository';
import ReleaseEventRepository from '@Repositories/ReleaseEventRepository';
import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import GlobalValues from '@Shared/GlobalValues';
import BasicEntryLinkStore from '@Stores/BasicEntryLinkStore';
import AdvancedSearchFilters from '@Stores/Search/AdvancedSearchFilters';
import { AlbumSortRule } from '@Stores/Search/AlbumSearchStore';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
import Ajv, { JSONSchemaType } from 'ajv';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

import IStoreWithPaging from '../IStoreWithPaging';
import AdvancedSearchFilter from '../Search/AdvancedSearchFilter';

interface AlbumCollectionRouteParams {
	advancedFilters?: AdvancedSearchFilter[];
	artistId?: number;
	collectionStatus?: string;
	discType?: string /* TODO: enum */;
	eventId?: number;
	filter?: string;
	page?: number;
	pageSize?: number;
	sort?: AlbumSortRule;
	tagId?: number;
	viewMode?: 'Details' | 'Tiles' /* TODO: enum */;
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<AlbumCollectionRouteParams> = require('./AlbumCollectionRouteParams.schema');
const validate = ajv.compile(schema);

export default class AlbumCollectionStore
	implements IStoreWithPaging<AlbumCollectionRouteParams> {
	public readonly advancedFilters = new AdvancedSearchFilters();
	@observable public albumType = AlbumType[AlbumType.Unknown] /* TODO: enum */;
	public readonly artist: BasicEntryLinkStore<ArtistContract>;
	@observable public artistName = '';
	@observable public collectionStatus = '';
	@observable public loading = true; // Currently loading for data
	@observable public page: AlbumForUserForApiContract[] = []; // Current page of items
	public readonly paging = new ServerSidePagingStore(20); // Paging store
	public pauseNotifications = false;
	public readonly releaseEvent: BasicEntryLinkStore<ReleaseEventContract>;
	@observable public searchTerm = '';
	@observable public sort = AlbumSortRule.Name;
	public readonly tag: BasicEntryLinkStore<TagBaseContract>;
	@observable public viewMode: 'Details' | 'Tiles' = 'Details'; /* TODO: enum */

	public constructor(
		private readonly values: GlobalValues,
		private readonly userRepo: UserRepository,
		artistRepo: ArtistRepository,
		eventRepo: ReleaseEventRepository,
		tagRepo: TagRepository,
		private readonly userId: number,
		public readonly publicCollection: boolean,
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

	@computed public get tagId(): number | undefined {
		return this.tag.id;
	}

	@computed public get tagName(): string | undefined {
		return this.tag.name;
	}

	@computed public get tagUrl(): string | undefined {
		return EntryUrlMapper.details_tag_contract(this.tag.entry);
	}

	@computed public get releaseEventUrl(): string {
		return EntryUrlMapper.details(
			EntryType.ReleaseEvent,
			this.releaseEvent.id!,
		);
	}

	public ratingStars = (userRating: number): { enabled: boolean }[] => {
		const ratings = _.map([1, 2, 3, 4, 5], (rating) => {
			return { enabled: Math.round(userRating) >= rating };
		});
		return ratings;
	};

	@action public updateResults = (clearResults: boolean = true): void => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;
		this.loading = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);

		this.userRepo
			.getAlbumCollectionList({
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
			})
			.then((result: PartialFindResultContract<AlbumForUserForApiContract>) => {
				this.pauseNotifications = false;

				runInAction(() => {
					if (pagingProperties.getTotalCount)
						this.paging.totalItems = result.totalCount;

					this.page = result.items;
					this.loading = false;
				});
			});
	};

	public popState = false;

	public readonly clearResultsByQueryKeys: (keyof AlbumCollectionRouteParams)[] = [
		'pageSize',
		'filter',
		'tagId',

		'advancedFilters',
		'discType',
		'artistId',
		'collectionStatus',
		'eventId',
	];

	@computed.struct public get routeParams(): AlbumCollectionRouteParams {
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
		};
	}
	public set routeParams(value: AlbumCollectionRouteParams) {
		this.advancedFilters.filters = value.advancedFilters ?? [];
		this.artist.id = value.artistId;
		this.collectionStatus = value.collectionStatus ?? '';
		this.albumType = value.discType ?? AlbumType[AlbumType.Unknown];
		this.releaseEvent.id = value.eventId;
		this.searchTerm = value.filter ?? '';
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 20;
		this.sort = value.sort ?? AlbumSortRule.Name;
		this.tag.id = value.tagId;
		this.viewMode = value.viewMode || 'Details';
	}

	public validateRouteParams = (
		data: any,
	): data is AlbumCollectionRouteParams => validate(data);
}
