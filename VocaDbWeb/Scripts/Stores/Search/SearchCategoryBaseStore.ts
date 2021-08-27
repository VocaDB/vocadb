import EntryWithTagUsagesContract from '@DataContracts/Base/EntryWithTagUsagesContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import IStoreWithRouteParams from '@Stores/IStoreWithRouteParams';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
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

import AdvancedSearchFilters from './AdvancedSearchFilters';
import { ICommonSearchStore } from './CommonSearchStore';
import { SearchRouteParams } from './SearchStore';
import TagFilter from './TagFilter';

export interface ISearchCategoryBaseStore
	extends IStoreWithRouteParams<SearchRouteParams> {
	updateResultsWithTotalCount: () => void;
}

// Base class for different types of searches.
export default abstract class SearchCategoryBaseStore<
	TEntry extends EntryWithTagUsagesContract
> implements ISearchCategoryBaseStore {
	public readonly advancedFilters = new AdvancedSearchFilters();
	private readonly commonSearchStore: ICommonSearchStore;
	@observable public loading = true; // Currently loading for data
	@observable public page: TEntry[] = []; // Current page of items
	public readonly paging = new ServerSidePagingStore(); // Paging store
	public pauseNotifications = false;

	public constructor(commonSearchStore: ICommonSearchStore) {
		makeObservable(this);

		this.commonSearchStore = commonSearchStore;

		reaction(
			() => commonSearchStore.pageSize,
			(pageSize) => {
				this.paging.pageSize = pageSize;
			},
		);
		reaction(
			() => this.paging.pageSize,
			(pageSize) => {
				commonSearchStore.pageSize = pageSize;
			},
		);
	}

	@computed public get childTags(): boolean {
		return this.commonSearchStore.tagFilters.childTags;
	}
	public set childTags(value: boolean) {
		this.commonSearchStore.tagFilters.childTags = value;
	}

	@computed public get draftsOnly(): boolean {
		return this.commonSearchStore.draftsOnly;
	}
	public set draftsOnly(value: boolean) {
		this.commonSearchStore.draftsOnly = value;
	}

	@computed public get pageSize(): number {
		return this.commonSearchStore.pageSize;
	}
	public set pageSize(value: number) {
		this.commonSearchStore.pageSize = value;
	}

	@computed public get searchTerm(): string {
		return this.commonSearchStore.searchTerm;
	}
	public set searchTerm(value: string) {
		this.commonSearchStore.searchTerm = value;
	}

	@computed public get showTags(): boolean {
		return this.commonSearchStore.showTags;
	}
	public set showTags(value: boolean) {
		this.showTags = value;
	}

	@computed public get tags(): TagFilter[] {
		return this.commonSearchStore.tagFilters.tags;
	}
	public set tags(value: TagFilter[]) {
		this.commonSearchStore.tagFilters.tags = value;
	}

	@computed public get tagIds(): number[] {
		return _.map(this.tags, (t) => t.id);
	}
	public set tagIds(value: number[]) {
		// OPTIMIZE
		this.commonSearchStore.tagFilters.tags = [];
		this.commonSearchStore.tagFilters.addTags(value);
	}

	public formatDate = (dateStr: string): string => {
		return moment(dateStr).utc().format('l');
	};

	// Method for loading a page of results.
	public abstract loadResults: (
		pagingProperties: PagingProperties,
		searchTerm: string,
		tags: number[],
		childTags: boolean,
		status?: string,
	) => Promise<PartialFindResultContract<TEntry>>;

	@action public selectTag = (tag: TagBaseContract): void => {
		this.tags = [TagFilter.fromContract(tag)];
	};

	public abstract routeParams: SearchRouteParams;

	@action public updateResults = (clearResults: boolean): void => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;
		this.loading = true;

		if (clearResults) this.paging.goToFirstPage();

		const pagingProperties = this.paging.getPagingProperties(clearResults);

		this.loadResults(
			pagingProperties,
			this.searchTerm,
			this.tagIds,
			this.childTags,
			this.draftsOnly ? 'Draft' : undefined,
		).then((result) => {
			if (this.showTags) {
				_.forEach(result.items, (item) => {
					if (item.tags) {
						item.tags = _.take(
							_.sortBy(item.tags, (t) => t.tag.name.toLowerCase()),
							10,
						);
					}
				});
			}

			this.pauseNotifications = false;

			runInAction(() => {
				if (pagingProperties.getTotalCount)
					this.paging.totalItems = result.totalCount;

				this.page = result.items;
				this.loading = false;
			});
		});
	};

	// Update results loading the first page and updating total number of items.
	// Commonly this is done after changing the filters or sorting.
	public updateResultsWithTotalCount = (): void => this.updateResults(true);

	public updateResultsWithoutTotalCount = (): void => this.updateResults(false);
}
