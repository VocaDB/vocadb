
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

	export class FeaturedSongListCategoryViewModel {
		
		constructor(private listRepo: rep.SongListRepository, private category: string) {

			// Should figure out a better way for this.
			this.showSort = category === "Concerts" || category === "VocaloidRanking";

			this.sort.subscribe(this.clear);

		}

		public clear = () => {
			this.lists([]);
			this.start = 0;
			this.loadMore();
		}

		public hasMore = ko.observable(false);

		private isInit = false;

		public init = () => {
			
			if (this.isInit)
				return;

			this.loadMore();
			this.isInit = true;

		}

		public lists = ko.observableArray<dc.SongListContract>([]);

		public loadMore = () => {
			this.listRepo.getFeatured(this.category, { start: this.start, maxEntries: 50, getTotalCount: true }, this.sort(), (result) => {
				ko.utils.arrayPushAll(this.lists, result.items);
				this.start = this.start + result.items.length;
				this.hasMore(result.totalCount > this.start);
			});
		};

		public showSort: boolean;

		public start = 0;

		public sort = ko.observable("Date");

	}

}