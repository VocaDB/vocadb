import UserApiContract from '@DataContracts/User/UserApiContract';
import UserGroup from '@Models/Users/UserGroup';
import UserRepository from '@Repositories/UserRepository';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
import debounceEffect from '@Stores/debounceEffect';
import { action, makeObservable, observable, reaction } from 'mobx';

export default class ListUsersStore {
	@observable public disabledUsers = false;
	@action public setDisabledUsers = (value: boolean): void => {
		this.disabledUsers = value;
	};

	@observable public group = UserGroup.Nothing;
	@action public setGroup = (value: UserGroup): void => {
		this.group = value;
	};

	@observable public loading = false;

	@observable public knowsLanguage = '';
	@action public setKnowsLanguage = (value: string): void => {
		this.knowsLanguage = value;
	};

	@observable public onlyVerifiedArtists = false;
	@action public setOnlyVerifiedArtists = (value: boolean): void => {
		this.onlyVerifiedArtists = value;
	};

	@observable public page: UserApiContract[] = []; // Current page of items
	@action public setPage = (value: UserApiContract[]): void => {
		this.page = value;
	};

	@observable public paging = new ServerSidePagingStore(20); // Paging view model
	public pauseNotifications = false;

	@observable public searchTerm = '';
	@action public setSearchTerm = (value: string): void => {
		this.searchTerm = value;
	};

	@observable public sort = 'RegisterDate' /* TODO: enum */;
	@action public setSort = (value: string): void => {
		this.sort = value;
	};

	public constructor(
		private readonly userRepo: UserRepository,
		searchTerm?: string,
		group?: UserGroup,
	) {
		makeObservable(this);

		if (group) this.group = group;

		this.searchTerm = searchTerm || '';

		reaction(() => this.disabledUsers, this.updateResultsWithTotalCount);
		reaction(() => this.group, this.updateResultsWithTotalCount);
		reaction(() => this.knowsLanguage, this.updateResultsWithTotalCount);
		reaction(() => this.onlyVerifiedArtists, this.updateResultsWithTotalCount);
		reaction(() => this.paging.page, this.updateResultsWithoutTotalCount);
		reaction(() => this.paging.pageSize, this.updateResultsWithTotalCount);
		reaction(
			() => this.searchTerm,
			debounceEffect(this.updateResultsWithTotalCount, 300),
		);
		reaction(() => this.sort, this.updateResultsWithoutTotalCount);

		this.updateResults(true);
	}

	private updateResults = (clearResults: boolean): void => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		if (clearResults) this.paging.setPage(1);

		const pagingProperties = this.paging.getPagingProperties(clearResults);
		this.userRepo
			.getList({
				paging: pagingProperties,
				query: this.searchTerm,
				sort: this.sort,
				groups: this.group,
				includeDisabled: this.disabledUsers,
				onlyVerified: this.onlyVerifiedArtists,
				knowsLanguage: this.knowsLanguage,
				nameMatchMode: 'Auto' /* TODO: enum */,
				fields: 'MainPicture' /* TODO: enum */,
			})
			.then((result) => {
				this.pauseNotifications = false;

				this.setPage(result.items);

				if (pagingProperties.getTotalCount)
					this.paging.setTotalItems(result.totalCount);
			});
	};

	public updateResultsWithTotalCount = (): void => this.updateResults(true);
	public updateResultsWithoutTotalCount = (): void => this.updateResults(false);
}
