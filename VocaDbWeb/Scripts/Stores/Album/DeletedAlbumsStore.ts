import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import {
	AlbumOptionalField,
	AlbumRepository,
} from '@/Repositories/AlbumRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { AlbumSortRule } from '@/Stores/Search/AlbumSearchStore';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import {
	includesAny,
	LocationStateStore,
	StateChangeEvent,
} from '@/route-sphere';
import Ajv from 'ajv';
import { computed, makeObservable, observable, runInAction } from 'mobx';

import schema from './DeletedAlbumsRouteParams.schema.json';

export interface DeletedAlbumsRouteParams {
	filter?: string;
	page?: number;
	pageSize?: number;
	sort?: AlbumSortRule;
}

const clearResultsByQueryKeys: (keyof DeletedAlbumsRouteParams)[] = [
	'pageSize',
	'filter',
	'sort',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<DeletedAlbumsRouteParams>(schema);

export class DeletedAlbumsStore
	implements LocationStateStore<DeletedAlbumsRouteParams> {
	@observable loading = false;
	@observable page: AlbumContract[] = []; // Current page of items
	readonly paging = new ServerSidePagingStore(20); // Paging view model
	@observable searchTerm = '';
	@observable showTags = false;
	@observable sort = AlbumSortRule.Name;
	@observable viewMode = 'Details' /* TODO: enum */;

	constructor(
		private readonly values: GlobalValues,
		private readonly albumRepo: AlbumRepository,
	) {
		makeObservable(this);
	}

	@computed.struct get locationState(): DeletedAlbumsRouteParams {
		return {
			filter: this.searchTerm,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			sort: this.sort,
		};
	}
	set locationState(value: DeletedAlbumsRouteParams) {
		this.searchTerm = value.filter ?? '';
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 20;
		this.sort = value.sort ?? AlbumSortRule.Name;
	}

	validateLocationState = (
		locationState: any,
	): locationState is DeletedAlbumsRouteParams => {
		return validate(locationState);
	};

	private updateResults = async (clearResults: boolean): Promise<void> => {
		this.loading = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);
		const result = await this.albumRepo.getList({
			paging: pagingProperties,
			lang: this.values.languagePreference,
			query: this.searchTerm,
			sort: this.sort,
			discTypes: undefined,
			tags: undefined,
			childTags: undefined,
			artistIds: undefined,
			artistParticipationStatus: undefined,
			childVoicebanks: undefined,
			includeMembers: undefined,
			fields: [
				AlbumOptionalField.AdditionalNames,
				AlbumOptionalField.MainPicture,
			],
			status: undefined,
			deleted: true,
			advancedFilters: undefined,
		});

		runInAction(() => {
			if (pagingProperties.getTotalCount)
				this.paging.totalItems = result.totalCount;

			this.page = result.items;
			this.loading = false;
		});
	};

	onLocationStateChange = (
		event: StateChangeEvent<DeletedAlbumsRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
