import { EntryWithTagUsagesContract } from '@/DataContracts/Base/EntryWithTagUsagesContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { AdvancedSearchFilters } from '@/Stores/Search/AdvancedSearchFilters';
import { ICommonSearchStore } from '@/Stores/Search/CommonSearchStore';
import { SearchRouteParams } from '@/Stores/Search/SearchStore';
import { TagFilter } from '@/Stores/Search/TagFilter';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import dayjs from '@/dayjs';
import { StateChangeEvent, LocationStateStore } from '@/route-sphere';
import UTC from 'dayjs/plugin/utc';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export interface ISearchCategoryBaseStore<
	TRouteParams extends SearchRouteParams
> extends Omit<LocationStateStore<TRouteParams>, 'validateLocationState'> {
	paging: ServerSidePagingStore;
	updateResults(clearResults: boolean): Promise<void>;
	updateResultsWithTotalCount(): Promise<void>;
}

dayjs.extend(UTC);

// Base class for different types of searches.
export abstract class SearchCategoryBaseStore<
	TRouteParams extends SearchRouteParams,
	TEntry extends EntryWithTagUsagesContract
> implements ISearchCategoryBaseStore<TRouteParams> {
	readonly advancedFilters = new AdvancedSearchFilters();
	private readonly commonSearchStore: ICommonSearchStore;
	@observable loading = true; // Currently loading for data
	@observable page: TEntry[] = []; // Current page of items
	readonly paging = new ServerSidePagingStore(); // Paging store

	constructor(commonSearchStore: ICommonSearchStore) {
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

	@computed get childTags(): boolean {
		return this.commonSearchStore.tagFilters.childTags;
	}
	set childTags(value: boolean) {
		this.commonSearchStore.tagFilters.childTags = value;
	}

	@computed get draftsOnly(): boolean {
		return this.commonSearchStore.draftsOnly;
	}
	set draftsOnly(value: boolean) {
		this.commonSearchStore.draftsOnly = value;
	}

	@computed get pageSize(): number {
		return this.commonSearchStore.pageSize;
	}
	set pageSize(value: number) {
		this.commonSearchStore.pageSize = value;
	}

	@computed get searchTerm(): string {
		return this.commonSearchStore.searchTerm;
	}
	set searchTerm(value: string) {
		this.commonSearchStore.searchTerm = value;
	}

	@computed get showTags(): boolean {
		return this.commonSearchStore.showTags;
	}
	set showTags(value: boolean) {
		this.commonSearchStore.showTags = value;
	}

	@computed get includedTags(): TagFilter[] {
		return this.commonSearchStore.tagFilters.tags.filter(t => !t.excluded);
	}
	set tags(value: TagFilter[]) {
		this.commonSearchStore.tagFilters.tags = value;
	}

	@computed get excludedTags(): TagFilter[] {
		return this.commonSearchStore.tagFilters.tags.filter(t => t.excluded);
	}

	@computed get tagIds(): number[] {
		return this.includedTags.map((t) => t.id);
	}
	set tagIds(value: number[]) {
		// OPTIMIZE
		this.commonSearchStore.tagFilters.tags = this.excludedTags;
		this.commonSearchStore.tagFilters.addTags(value);
	}
	
	@computed get excludedTagIds(): number[] {
		return this.excludedTags.map((t) => t.id);
	}
	set excludedTagIds(value: number[]) {
		// OPTIMIZE
		this.commonSearchStore.tagFilters.tags = this.includedTags;
		this.commonSearchStore.tagFilters.addTags(value, true);
	}

	formatDate = (dateStr: string): string => {
		return dayjs(dateStr).utc().format('l');
	};

	// Method for loading a page of results.
	abstract loadResults: (
		pagingProperties: PagingProperties,
	) => Promise<PartialFindResultContract<TEntry>>;

	@action selectTag = (tag: TagBaseContract): void => {
		this.tags = [TagFilter.fromContract(tag)];
	};

	abstract locationState: TRouteParams;

	private pauseNotifications = false;

	@action updateResults = async (clearResults: boolean): Promise<void> => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;
		this.loading = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);

		const result = await this.loadResults(pagingProperties);

		if (this.showTags) {
			for (const item of result.items) {
				if (item.tags) {
					item.tags = item.tags
						.sortBy((t) => t.tag.name.toLowerCase())
						.take(10);
				}
			}
		}

		this.pauseNotifications = false;

		runInAction(() => {
			if (pagingProperties.getTotalCount)
				this.paging.totalItems = result.totalCount;

			this.page = result.items;
			this.loading = false;
		});
	};

	// Update results loading the first page and updating total number of items.
	// Commonly this is done after changing the filters or sorting.
	updateResultsWithTotalCount = (): Promise<void> => {
		return this.updateResults(true);
	};

	updateResultsWithoutTotalCount = (): Promise<void> => {
		return this.updateResults(false);
	};

	abstract onLocationStateChange({
		keys,
		popState,
	}: StateChangeEvent<TRouteParams>): void;
}
