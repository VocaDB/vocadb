
module vdb.viewModels.songList {
	
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class FeaturedSongListsViewModel {

		constructor(listRepo: rep.SongListRepository,
			resourceRepo: rep.ResourceRepository,
			tagRepo: rep.TagRepository,
			languageSelection: string,
			cultureCode: string,
			tagIds: number[],
			categoryNames: string[]) {
			
			_.forEach(categoryNames, (categoryName) => {
				this.categories[categoryName] = new FeaturedSongListCategoryViewModel(listRepo, resourceRepo, tagRepo, languageSelection, cultureCode, tagIds, categoryName);
			});

			window.onhashchange = () => {
				if (window.location.hash && window.location.hash.length >= 1)
					this.setView(window.location.hash.substr(1));
			};

		}

		public categories: { [index: string]: FeaturedSongListCategoryViewModel; } = {};

		public view = ko.observable("Concerts");

		public setView = (viewName: string) => {

			if (!viewName)
				viewName = "Concerts";

			window.scrollTo(0, 0);
			window.location.hash = viewName;
			this.view(viewName);

		}

	}

	export class FeaturedSongListCategoryViewModel extends SongListsBaseViewModel {

		constructor(private listRepo: rep.SongListRepository,
			resourceRepo: rep.ResourceRepository,
			tagRepo: rep.TagRepository,
			languageSelection: string,
			cultureCode: string,
			tagIds: number[],
			private category: string) {

			// Should figure out a better way for this.
			super(resourceRepo, tagRepo, languageSelection, cultureCode, tagIds, category === "Concerts" || category === "VocaloidRanking");

		}

		public loadMoreItems = (callback: (result: dc.PartialFindResultContract<dc.SongListContract>) => void) => {
			this.listRepo.getFeatured(this.query(), this.category, { start: this.start, maxEntries: 50, getTotalCount: true }, this.tagFilters.tagIds(), this.sort(), callback);
		};

	}

}