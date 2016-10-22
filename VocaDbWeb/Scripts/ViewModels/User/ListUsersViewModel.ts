
module vdb.viewModels.user {
	
	import dc = vdb.dataContracts;

	export class ListUsersViewModel {
		
		constructor(
			private repo: repositories.UserRepository,
			resourceRepo: repositories.ResourceRepository,
			cultureCode: string,
			searchTerm: string) {

			this.searchTerm = ko.observable(searchTerm || "").extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });

			this.resources = new vdb.models.ResourcesManager(resourceRepo, cultureCode);
			this.resources.loadResources(null, "userGroupNames");

			this.disabledUsers.subscribe(this.updateResultsWithTotalCount);
			this.group.subscribe(this.updateResultsWithTotalCount);
			this.knowsLanguage.subscribe(this.updateResultsWithTotalCount);
			this.onlyVerifiedArtists.subscribe(this.updateResultsWithTotalCount);
			this.paging.page.subscribe(this.updateResultsWithoutTotalCount);
			this.paging.pageSize.subscribe(this.updateResultsWithTotalCount);
			this.searchTerm.subscribe(this.updateResultsWithTotalCount);
			this.sort.subscribe(this.updateResultsWithoutTotalCount);

			this.updateResults(true);

		}

		public disabledUsers = ko.observable(false);
		public group = ko.observable("Nothing");
		public loading = ko.observable(false);
		public knowsLanguage = ko.observable("");
		public onlyVerifiedArtists = ko.observable(false);
		public page = ko.observableArray<dc.user.UserApiContract>([]); // Current page of items
		public paging = new ServerSidePagingViewModel(20); // Paging view model
		public pauseNotifications = false;
		public resources: vdb.models.ResourcesManager;
		public searchTerm: KnockoutObservable<string>;
		public sort = ko.observable("RegisterDate");

		public userGroupName = (userGroup: string) => {
			return (this.resources.resources().userGroupNames != null ? this.resources.resources().userGroupNames[userGroup] : userGroup);
		}

		public updateResultsWithTotalCount = () => this.updateResults(true);
		public updateResultsWithoutTotalCount = () => this.updateResults(false);

		private updateResults = (clearResults: boolean) => {

			// Disable duplicate updates
			if (this.pauseNotifications)
				return;

			this.pauseNotifications = true;

			if (clearResults)
				this.paging.page(1);

			var pagingProperties = this.paging.getPagingProperties(clearResults);
			this.repo.getList(pagingProperties, this.searchTerm(), this.sort(), this.group(), this.disabledUsers(),
				this.onlyVerifiedArtists(), this.knowsLanguage(), "Auto", "MainPicture", result => {

				this.pauseNotifications = false;

				this.page(result.items);

				if (pagingProperties.getTotalCount)
					this.paging.totalItems(result.totalCount);

			});

		}

	}

}