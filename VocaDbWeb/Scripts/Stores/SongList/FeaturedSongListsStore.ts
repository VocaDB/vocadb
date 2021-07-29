import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import SongListContract from '@DataContracts/Song/SongListContract';
import SongListRepository from '@Repositories/SongListRepository';
import TagRepository from '@Repositories/TagRepository';
import GlobalValues from '@Shared/GlobalValues';
import _ from 'lodash';
import { action, makeObservable, observable } from 'mobx';

import SongListsBaseStore from './SongListsBaseStore';

export enum SongListFeaturedCategory {
	Nothing = 'Nothing',
	Concerts = 'Concerts',
	VocaloidRanking = 'VocaloidRanking',
	Pools = 'Pools',
	Other = 'Other',
}

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

export default class FeaturedSongListsStore {
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

		_.forEach(categoryNames, (categoryName) => {
			this.categories[categoryName] = new FeaturedSongListCategoryStore(
				values,
				songListRepo,
				tagRepo,
				tagIds,
				categoryName,
			);
		});

		// TODO: find a replacement for window.onhashchange.
	}

	@action public setCategory = (
		categoryName: SongListFeaturedCategory,
	): void => {
		if (!categoryName) categoryName = SongListFeaturedCategory.Concerts;

		this.category = categoryName;
	};
}
