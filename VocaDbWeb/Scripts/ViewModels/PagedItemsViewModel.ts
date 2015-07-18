
module vdb.viewModels {
	
	import dc = vdb.dataContracts;

	// Generic viewmodel that supports simple paging by loading more items
	export class PagedItemsViewModel<TModel> {
		
		public clear = () => {
			this.items([]);
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

		public items = ko.observableArray<TModel>([]);

		public loadMore = () => {
			this.loadMoreItems(this.itemsLoaded);
		}

		public loadMoreItems = (callback: (result: dc.PartialFindResultContract<TModel>) => void) => { };

		public itemsLoaded = (result: dc.PartialFindResultContract<TModel>) => {
			ko.utils.arrayPushAll(this.items, result.items);
			this.start = this.start + result.items.length;
			this.hasMore(result.totalCount > this.start);			
		}

		public start = 0;

	}

} 