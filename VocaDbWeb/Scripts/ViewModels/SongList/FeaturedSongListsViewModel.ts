
module vdb.viewModels.songList {
	
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class FeaturedSongListsViewModel {

		constructor(listRepo: rep.SongListRepository, resourceRepo: rep.ResourceRepository, cultureCode: string, categoryNames: string[]) {
			
			_.forEach(categoryNames, (categoryName) => {
				this.categories[categoryName] = new FeaturedSongListCategoryViewModel(listRepo, resourceRepo, cultureCode, categoryName);
			});

		}

		public categories: { [index: string]: FeaturedSongListCategoryViewModel; } = {};

	}

	export class FeaturedSongListCategoryViewModel extends SongListsBaseViewModel {

		constructor(private listRepo: rep.SongListRepository, resourceRepo: rep.ResourceRepository, cultureCode: string, private category: string) {

			// Should figure out a better way for this.
			super(resourceRepo, cultureCode, category === "Concerts" || category === "VocaloidRanking");

		}

		public loadMoreItems = (callback: (result: dc.PartialFindResultContract<dc.SongListContract>) => void) => {
			this.listRepo.getFeatured(this.query(), this.category, { start: this.start, maxEntries: 50, getTotalCount: true }, this.sort(), callback);
		};

	}

}