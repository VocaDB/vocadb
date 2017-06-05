
namespace vdb.viewModels.search {

	import dc = vdb.dataContracts;

	export class EventSearchViewModel extends SearchCategoryBaseViewModel<dc.ReleaseEventContract> {

		constructor(
			searchViewModel: SearchViewModel,
			lang: vdb.models.globalization.ContentLanguagePreference,
			private eventRepo: rep.ReleaseEventRepository,
			public loggedUserId?: number) {

			super(searchViewModel);

			this.category.subscribe(this.updateResultsWithTotalCount);
			this.onlyMyEvents.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithTotalCount);

			this.loadResults = (pagingProperties, searchTerm, tag, childTags, status, callback) => {

				this.eventRepo.getList({
					start: pagingProperties.start, maxResults: pagingProperties.maxEntries, getTotalCount: pagingProperties.getTotalCount,
					lang: lang, query: searchTerm, sort: this.sort(), category: this.category() === 'Unspecified' ? null : this.category(),
					childTags: childTags, tagIds: tag,
					userCollectionId: this.onlyMyEvents() ? loggedUserId : null,
					status: status,
					fields: "AdditionalNames,MainPicture,Series"
				}, callback);

			}

			this.sortName = ko.computed(() => {
				return searchViewModel.resourcesManager.resources().eventSortRuleNames != null ? searchViewModel.resourcesManager.resources().eventSortRuleNames[this.sort()] : "";
			});

		}

		public allowAliases = ko.observable(false);
		public category = ko.observable("");
		public onlyMyEvents = ko.observable(false);
		public sort = ko.observable("Name");
		public sortName: KnockoutComputed<string>;

		public getCategoryName = (event: dc.ReleaseEventContract) => {

			var inherited = event.series ? event.series.category : event.category;

			if (!inherited || inherited === 'Unspecified')
				return '';

			return this.searchViewModel.resourcesManager.resources().eventCategoryNames != null ? this.searchViewModel.resourcesManager.resources().eventCategoryNames[inherited] : "";;

		}

	}

}