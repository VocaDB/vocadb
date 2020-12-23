import ArtistFilters from './ArtistFilters';
import ArtistRepository from '../../Repositories/ArtistRepository';
import ContentLanguagePreference from '../../Models/Globalization/ContentLanguagePreference';
import ReleaseEventContract from '../../DataContracts/ReleaseEvents/ReleaseEventContract';
import ReleaseEventRepository from '../../Repositories/ReleaseEventRepository';
import SearchCategoryBaseViewModel from './SearchCategoryBaseViewModel';
import SearchViewModel from './SearchViewModel';

	export default class EventSearchViewModel extends SearchCategoryBaseViewModel<ReleaseEventContract> {

		constructor(
			searchViewModel: SearchViewModel,
			lang: ContentLanguagePreference,
			private readonly eventRepo: ReleaseEventRepository,
			artistRepo: ArtistRepository,
			public loggedUserId: number,
			sort: string,
			artistId: number[],
			category: string) {

			super(searchViewModel);

			this.artistFilters = new ArtistFilters(artistRepo, false);
			this.artistFilters.selectArtists(artistId);

			if (sort)
				this.sort(sort);

			if (category)
				this.category(category);

			this.afterDate.subscribe(this.updateResultsWithTotalCount);
			this.artistFilters.filters.subscribe(this.updateResultsWithTotalCount);
			this.beforeDate.subscribe(this.updateResultsWithTotalCount);
			this.category.subscribe(this.updateResultsWithTotalCount);
			this.onlyMyEvents.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithTotalCount);

			this.loadResults = (pagingProperties, searchTerm, tag, childTags, status, callback) => {

				this.eventRepo.getList({
					start: pagingProperties.start, maxResults: pagingProperties.maxEntries, getTotalCount: pagingProperties.getTotalCount,
					lang: lang, query: searchTerm, sort: this.sort(), category: this.category() === 'Unspecified' ? null : this.category(),
					childTags: childTags, tagIds: tag,
					userCollectionId: this.onlyMyEvents() ? loggedUserId : null,
					artistId: this.artistFilters.artistIds(),
					childVoicebanks: this.artistFilters.childVoicebanks(),
					includeMembers: this.artistFilters.includeMembers(),
					afterDate: this.afterDate(),
					beforeDate: this.beforeDate(),
					status: status,
					fields: this.fields()
				}, callback);

			}

			this.sortName = ko.computed(() => {
				return searchViewModel.resourcesManager.resources().eventSortRuleNames != null ? searchViewModel.resourcesManager.resources().eventSortRuleNames[this.sort()] : "";
			});

		}

		public afterDate = ko.observable<Date>(null);		
		public allowAliases = ko.observable(false);
		public artistFilters: ArtistFilters;
		public beforeDate = ko.observable<Date>(null);
		public category = ko.observable("");
		public onlyMyEvents = ko.observable(false);
		public sort = ko.observable("Name");
		public sortName: KnockoutComputed<string>;
		
		public fields = ko.computed(() => this.searchViewModel.showTags() ? "AdditionalNames,MainPicture,Series,Venue,Tags" : "AdditionalNames,MainPicture,Series,Venue");

		public getCategoryName = (event: ReleaseEventContract) => {

			var inherited = event.series ? event.series.category : event.category;

			if (!inherited || inherited === 'Unspecified')
				return '';

			return this.searchViewModel.resourcesManager.resources().eventCategoryNames != null ? this.searchViewModel.resourcesManager.resources().eventCategoryNames[inherited] : "";;

		}

	}