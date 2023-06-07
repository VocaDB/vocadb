import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { SongListContract } from '@/DataContracts/Song/SongListContract';
import { SongListFeaturedCategory } from '@/Models/SongLists/SongListFeaturedCategory';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import {
	SongListsBaseStore,
	SongListSortRule,
} from '@/Stores/SongList/SongListsBaseStore';
import {
	includesAny,
	StateChangeEvent,
	LocationStateStore,
} from '@/route-sphere';
import Ajv from 'ajv';
import { action, computed, makeObservable, observable } from 'mobx';

import schema from './FeaturedSongListsRouteParams.schema.json';

export class FeaturedSongListCategoryStore extends SongListsBaseStore {
	constructor(
		values: GlobalValues,
		private readonly songListRepo: SongListRepository,
		tagRepo: TagRepository,
		tagIds: number[],
		private readonly category: SongListFeaturedCategory,
	) {
		// Should figure out a better way for this.
		super(
			values,
			tagRepo,
			tagIds,
			category === SongListFeaturedCategory.Concerts ||
				category === SongListFeaturedCategory.VocaloidRanking,
		);

		makeObservable(this);
	}

	loadMoreItems = (): Promise<PartialFindResultContract<SongListContract>> => {
		return this.songListRepo.getFeatured({
			query: this.query,
			category: this.category,
			paging: { start: this.start, maxEntries: 50, getTotalCount: true },
			tagIds: this.tagFilters.tagIds,
			fields: this.fields,
			sort: this.sort,
		});
	};
}

interface FeaturedSongListsRouteParams {
	categoryName?: SongListFeaturedCategory;
	filter?: string;
	sort?: SongListSortRule;
	tagId?: number | number[];
}

const clearResultsByQueryKeys: (keyof FeaturedSongListsRouteParams)[] = [
	'filter',
	'sort',
	'tagId',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<FeaturedSongListsRouteParams>(schema);

export class FeaturedSongListsStore
	implements LocationStateStore<FeaturedSongListsRouteParams> {
	categories: { [index: string]: FeaturedSongListCategoryStore } = {};
	@observable category = SongListFeaturedCategory.Concerts;

	constructor(
		values: GlobalValues,
		songListRepo: SongListRepository,
		tagRepo: TagRepository,
		tagIds: number[],
		categoryNames: SongListFeaturedCategory[],
	) {
		makeObservable(this);

		for (const categoryName of categoryNames) {
			this.categories[categoryName] = new FeaturedSongListCategoryStore(
				values,
				songListRepo,
				tagRepo,
				tagIds,
				categoryName,
			);
		}
	}

	@action setCategory = (categoryName: SongListFeaturedCategory): void => {
		if (!categoryName) categoryName = SongListFeaturedCategory.Concerts;

		this.category = categoryName;
	};

	private get currentCategoryStore(): FeaturedSongListCategoryStore {
		return this.categories[this.category];
	}

	@computed.struct get locationState(): FeaturedSongListsRouteParams {
		return {
			categoryName: this.category,
			filter: this.currentCategoryStore.query,
			sort: this.currentCategoryStore.sort,
			tagId: this.currentCategoryStore.tagIds,
		};
	}
	set locationState(value: FeaturedSongListsRouteParams) {
		this.category = value.categoryName ?? SongListFeaturedCategory.Concerts;
		this.currentCategoryStore.query = value.filter ?? '';
		this.currentCategoryStore.sort =
			value.sort ??
			(this.category === SongListFeaturedCategory.Other ||
			this.category === SongListFeaturedCategory.Pools
				? SongListSortRule.Name
				: SongListSortRule.Date);
		this.currentCategoryStore.tagIds = ([] as number[]).concat(
			value.tagId ?? [],
		);
	}

	validateLocationState = (data: any): data is FeaturedSongListsRouteParams => {
		return validate(data);
	};

	private pauseNotifications = false;

	updateResults = async (clearResults: boolean): Promise<void> => {
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		await this.currentCategoryStore.clear();

		this.pauseNotifications = false;
	};

	onLocationStateChange = (
		event: StateChangeEvent<FeaturedSongListsRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		this.updateResults(clearResults);
	};
}
