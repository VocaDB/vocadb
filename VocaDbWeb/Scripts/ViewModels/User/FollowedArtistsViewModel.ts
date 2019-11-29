
//module vdb.viewModels.user {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class FollowedArtistsViewModel {

		constructor(private userRepo: rep.UserRepository,
			private resourceRepo: rep.ResourceRepository,
			tagRepo: rep.TagRepository,
			private languageSelection: string, private loggedUserId: number, private cultureCode: string) {

			this.tagFilters = new viewModels.search.TagFilters(tagRepo, languageSelection);

			this.paging.page.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.pageSize.subscribe(this.updateResultsWithTotalCount);
			this.artistType.subscribe(this.updateResultsWithTotalCount);
			this.tagFilters.tags.subscribe(this.updateResultsWithTotalCount);

		}

		public init = () => {

			if (this.isInit)
				return;

			this.resourceRepo.getList(this.cultureCode, ['artistTypeNames'], resources => {
				this.resources(resources);
				this.updateResultsWithTotalCount();
				this.isInit = true;
			});

		};

		public artistType = ko.observable("Unknown");
		public isInit = false;
		public loading = ko.observable(true); // Currently loading for data
		public page = ko.observableArray<dc.RatedSongForUserForApiContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(20); // Paging view model
		public pauseNotifications = false;
		public resources = ko.observable<any>();
		public tagFilters: viewModels.search.TagFilters;

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		public updateResults = (clearResults: boolean = true) => {

			// Disable duplicate updates
			if (this.pauseNotifications)
				return;

			this.pauseNotifications = true;
			this.loading(true);

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);

			this.userRepo.getFollowedArtistsList(this.loggedUserId, pagingProperties, this.languageSelection,
				this.tagFilters.tagIds(),
				this.artistType(),
				(result: any) => {

					this.pauseNotifications = false;

					if (pagingProperties.getTotalCount)
						this.paging.totalItems(result.totalCount);

					this.page(result.items);
					this.loading(false);

				});

		}

	}

//}