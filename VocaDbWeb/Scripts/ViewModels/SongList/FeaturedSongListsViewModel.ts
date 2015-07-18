
module vdb.viewModels.songList {
	
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class FeaturedSongListsViewModel {
		
		constructor(listRepo: rep.SongListRepository, categoryNames: string[]) {
			
			_.forEach(categoryNames, (categoryName) => {
				this.categories[categoryName] = new FeaturedSongListCategoryViewModel(listRepo, categoryName);
			});

			this.initCategory(categoryNames[0]);

		}

		public categories: { [index: string]: FeaturedSongListCategoryViewModel; } = {};

		public initCategory = (category: string) => {
			this.categories[category].init();
		}

	}

	export class FeaturedSongListCategoryViewModel extends PagedItemsViewModel<dc.SongListContract> {
		
		constructor(private listRepo: rep.SongListRepository, private category: string) {

			super();

			// Should figure out a better way for this.
			this.showSort = category === "Concerts" || category === "VocaloidRanking";

			this.sort.subscribe(this.clear);

		}

		public isFirstForYear = (current: dc.SongListContract, index: number) => {

			if (this.sort() !== "Date")
				return false;

			if (!current.eventDate)
				return false;

			if (index === 0)
				return true;

			var prev = this.items()[index - 1];

			if (!prev.eventDate)
				return false;

			var currentYear = moment(current.eventDate).year();
			var prevYear = moment(prev.eventDate).year();

			return currentYear !== prevYear;
				
		}

		public loadMoreItems = (callback: (result: dc.PartialFindResultContract<dc.SongListContract>) => void) => {
			this.listRepo.getFeatured(this.category, { start: this.start, maxEntries: 50, getTotalCount: true }, this.sort(), callback);
		};

		public showSort: boolean;

		public sort = ko.observable("Date");

	}

}