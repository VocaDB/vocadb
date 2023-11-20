import { GlobalValues } from '@/Shared/GlobalValues';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import {
	includesAny,
	LocationStateStore,
	StateChangeEvent,
} from '@/route-sphere';
import Ajv from 'ajv';
import { computed, makeObservable, observable, runInAction } from 'mobx';

import schema from './DeletedTagsRouteParams.schema.json';
import { TagSortRule } from '../Search/TagSearchStore';
import { TagApiContract } from '@/DataContracts/Tag/TagApiContract';
import { TagRepository } from '@/vdb';

export interface DeletedTagsRouteParams {
	filter?: string;
	page?: number;
	pageSize?: number;
	sort?: TagSortRule;
}

const clearResultsByQueryKeys: (keyof DeletedTagsRouteParams)[] = [
	'pageSize',
	'filter',
	'sort',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<DeletedTagsRouteParams>(schema);

export class DeletedTagsStore
	implements LocationStateStore<DeletedTagsRouteParams> {
	@observable loading = false;
	@observable page: TagApiContract[] = []; // Current page of items
	readonly paging = new ServerSidePagingStore(20); // Paging view model
	@observable searchTerm = '';
	@observable sort = TagSortRule.Name;

	constructor(
		private readonly values: GlobalValues,
		private readonly tagRepo: TagRepository,
	) {
		makeObservable(this);
	}

	@computed.struct get locationState(): DeletedTagsRouteParams {
		return {
			filter: this.searchTerm,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			sort: this.sort,
		};
	}
	set locationState(value: DeletedTagsRouteParams) {
		this.searchTerm = value.filter ?? '';
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 20;
		this.sort = value.sort ?? TagSortRule.Name;
	}

	validateLocationState = (
		locationState: any,
	): locationState is DeletedTagsRouteParams => {
		return validate(locationState);
	};

	private updateResults = async (clearResults: boolean): Promise<void> => {
		this.loading = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);
		const result = await this.tagRepo.getList({
            queryParams: {
                deleted: true,
                query: this.searchTerm,
                sort: this.sort,
                start: pagingProperties.start,
                maxResults: pagingProperties.maxEntries,
                lang: this.values.languagePreference,
                getTotalCount: true
            },
		});

		runInAction(() => {
			if (pagingProperties.getTotalCount)
				this.paging.totalItems = result.totalCount;

			this.page = result.items;
			this.loading = false;
		});
	};

	onLocationStateChange = (
		event: StateChangeEvent<DeletedTagsRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
