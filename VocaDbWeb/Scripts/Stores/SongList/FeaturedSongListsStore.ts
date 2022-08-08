import PartialFindResultContract from '@/DataContracts/PartialFindResultContract';
import SongListContract from '@/DataContracts/Song/SongListContract';
import SongListFeaturedCategory from '@/Models/SongLists/SongListFeaturedCategory';
import SongListRepository from '@/Repositories/SongListRepository';
import TagRepository from '@/Repositories/TagRepository';
import GlobalValues from '@/Shared/GlobalValues';
import { StoreWithUpdateResults } from '@vocadb/route-sphere';
import Ajv, { JSONSchemaType } from 'ajv';
import { action, computed, makeObservable, observable } from 'mobx';

import SongListsBaseStore, { SongListSortRule } from './SongListsBaseStore';

export class FeaturedSongListCategoryStore extends SongListsBaseStore {
	public constructor(
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

	public loadMoreItems = (): Promise<
		PartialFindResultContract<SongListContract>
	> => {
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

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<FeaturedSongListsRouteParams> = require('./FeaturedSongListsRouteParams.schema');
const validate = ajv.compile(schema);

export default class FeaturedSongListsStore
	implements StoreWithUpdateResults<FeaturedSongListsRouteParams> {
	public categories: { [index: string]: FeaturedSongListCategoryStore } = {};
	@observable public category = SongListFeaturedCategory.Concerts;

	public constructor(
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

	@action public setCategory = (
		categoryName: SongListFeaturedCategory,
	): void => {
		if (!categoryName) categoryName = SongListFeaturedCategory.Concerts;

		this.category = categoryName;
	};

	public popState = false;

	public clearResultsByQueryKeys: (keyof FeaturedSongListsRouteParams)[] = [
		'filter',
		'sort',
		'tagId',
	];

	private get currentCategoryStore(): FeaturedSongListCategoryStore {
		return this.categories[this.category];
	}

	@computed.struct public get routeParams(): FeaturedSongListsRouteParams {
		return {
			categoryName: this.category,
			filter: this.currentCategoryStore.query,
			sort: this.currentCategoryStore.sort,
			tagId: this.currentCategoryStore.tagIds,
		};
	}
	public set routeParams(value: FeaturedSongListsRouteParams) {
		this.category = value.categoryName ?? SongListFeaturedCategory.Concerts;
		this.currentCategoryStore.query = value.filter ?? '';
		this.currentCategoryStore.sort = value.sort ?? SongListSortRule.Date;
		this.currentCategoryStore.tagIds = ([] as number[]).concat(
			value.tagId ?? [],
		);
	}

	public validateRouteParams = (
		data: any,
	): data is FeaturedSongListsRouteParams => {
		return validate(data);
	};

	private pauseNotifications = false;

	public updateResults = async (clearResults: boolean): Promise<void> => {
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		await this.currentCategoryStore.clear();

		this.pauseNotifications = false;
	};
}
