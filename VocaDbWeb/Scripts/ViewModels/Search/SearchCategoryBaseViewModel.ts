
module vdb.viewModels.search {

	import cls = vdb.models;
	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export interface ISearchCategoryBaseViewModel {

		updateResultsWithTotalCount: () => void;

	}

	// Base class for different types of searches.
	export class SearchCategoryBaseViewModel<TEntry> implements ISearchCategoryBaseViewModel {

		constructor(public searchViewModel: SearchViewModel) {

			if (searchViewModel) {
				this.draftsOnly = searchViewModel.draftsOnly;
				this.searchTerm = searchViewModel.searchTerm;
				this.showTags = searchViewModel.showTags;
				this.tag = searchViewModel.tag;
				searchViewModel.pageSize.subscribe(pageSize => this.paging.pageSize(pageSize));				
				this.paging.pageSize.subscribe(pageSize => searchViewModel.pageSize(pageSize));
			} else {
				this.draftsOnly = ko.observable(false);
				this.searchTerm = ko.observable("");
				this.showTags = ko.observable(false);
				this.tag = ko.observable("");
				this.draftsOnly.subscribe(this.updateResultsWithTotalCount);
				this.searchTerm.subscribe(this.updateResultsWithTotalCount);
				this.showTags.subscribe(this.updateResultsWithoutTotalCount);
				this.tag.subscribe(this.updateResultsWithTotalCount);
			}

			this.paging.page.subscribe(this.updateResultsWithoutTotalCount);

		}

		public draftsOnly: KnockoutObservable<boolean>;

		// Method for loading a page of results.
		public loadResults: (pagingProperties: dc.PagingProperties, searchTerm: string, tag: string,
			status: string, callback: (result: any) => void) => void;

		public loading = ko.observable(true); // Currently loading for data

		public page = ko.observableArray<dc.EntryContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(); // Paging view model
		public pauseNotifications = false;

		public searchTerm: KnockoutObservable<string>;

		public showTags: KnockoutObservable<boolean>;

		public tag: KnockoutObservable<string>;

		// Update results loading the first page and updating total number of items.
		// Commonly this is done after changing the filters or sorting.
		public updateResultsWithTotalCount = () => this.updateResults(true);

		// Update a new page of results. Does not update total number of items.
		// This assumes the filters have not changed. Commonly this is done when paging.
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean) => {

			// Disable duplicate updates
			if (this.pauseNotifications)
				return;

			this.pauseNotifications = true;
			this.loading(true);

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			this.loadResults(pagingProperties, this.searchTerm(), this.tag(),
				this.draftsOnly() ? "Draft" : null, (result: any) => {

					if (this.showTags()) {

						_.forEach(result.items, (item: dc.EntryWithTagUsagesContract) => {

							if (item.tags)
								item.tags = _.first(_.sortBy(item.tags, t => t.name.toLowerCase()), 10);

						});

					}

					this.pauseNotifications = false;

					if (pagingProperties.getTotalCount)
						this.paging.totalItems(result.totalCount);

					this.page(result.items);
					this.loading(false);

				});

		};

	}

}