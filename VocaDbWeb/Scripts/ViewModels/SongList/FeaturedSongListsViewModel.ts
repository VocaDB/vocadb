import PartialFindResultContract from '@/DataContracts/PartialFindResultContract';
import SongListContract from '@/DataContracts/Song/SongListContract';
import ResourceRepository from '@/Repositories/ResourceRepository';
import SongListRepository from '@/Repositories/SongListRepository';
import TagRepository from '@/Repositories/TagRepository';
import GlobalValues from '@/Shared/GlobalValues';
import SongListsBaseViewModel from '@/ViewModels/SongList/SongListsBaseViewModel';
import ko from 'knockout';

export default class FeaturedSongListsViewModel {
	public constructor(
		values: GlobalValues,
		listRepo: SongListRepository,
		resourceRepo: ResourceRepository,
		tagRepo: TagRepository,
		tagIds: number[],
		categoryNames: string[],
	) {
		for (const categoryName of categoryNames) {
			this.categories[categoryName] = new FeaturedSongListCategoryViewModel(
				values,
				listRepo,
				resourceRepo,
				tagRepo,
				tagIds,
				categoryName,
			);
		}

		window.onhashchange = (): void => {
			if (window.location.hash && window.location.hash.length >= 1)
				this.setCategory(window.location.hash.substr(1));
		};
	}

	public categories: {
		[index: string]: FeaturedSongListCategoryViewModel;
	} = {};

	public category = ko.observable('Concerts');

	public setCategory = (categoryName: string): void => {
		if (!categoryName) categoryName = 'Concerts';

		window.scrollTo(0, 0);
		window.location.hash = categoryName;
		this.category(categoryName);
	};
}

export class FeaturedSongListCategoryViewModel extends SongListsBaseViewModel {
	public constructor(
		values: GlobalValues,
		private listRepo: SongListRepository,
		resourceRepo: ResourceRepository,
		tagRepo: TagRepository,
		tagIds: number[],
		private category: string,
	) {
		// Should figure out a better way for this.
		super(
			values,
			resourceRepo,
			tagRepo,
			tagIds,
			category === 'Concerts' || category === 'VocaloidRanking',
		);
	}

	public loadMoreItems = (
		callback: (result: PartialFindResultContract<SongListContract>) => void,
	): void => {
		this.listRepo
			.getFeatured({
				query: this.query(),
				category: this.category,
				paging: { start: this.start, maxEntries: 50, getTotalCount: true },
				tagIds: this.tagFilters.tagIds(),
				fields: this.fields(),
				sort: this.sort(),
			})
			.then(callback);
	};
}
