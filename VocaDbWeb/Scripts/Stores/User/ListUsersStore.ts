import UserApiContract from '@DataContracts/User/UserApiContract';
import UserGroup from '@Models/Users/UserGroup';
import UserRepository from '@Repositories/UserRepository';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
import debounceEffect from '@Stores/debounceEffect';
import { makeObservable, observable, reaction, runInAction } from 'mobx';

// Corresponds to the UserSortRule enum in C#.
export enum UserSortRule {
	RegisterDate = 'RegisterDate',
	Name = 'Name',
	Group = 'Group',
}

export default class ListUsersStore {
	@observable public disabledUsers = false;
	@observable public group = UserGroup.Nothing;
	@observable public loading = false;
	@observable public knowsLanguage = '';
	@observable public onlyVerifiedArtists = false;
	@observable public page: UserApiContract[] = []; // Current page of items
	@observable public paging = new ServerSidePagingStore(20); // Paging view model
	public pauseNotifications = false;
	@observable public searchTerm = '';
	@observable public sort = UserSortRule.RegisterDate;

	public constructor(private readonly userRepo: UserRepository) {
		makeObservable(this);

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

		if (clearResults) this.paging.goToFirstPage();

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

				runInAction(() => {
					this.page = result.items;

					if (pagingProperties.getTotalCount)
						this.paging.totalItems = result.totalCount;
				});
			});
	};

	public updateResultsWithTotalCount = (): void => this.updateResults(true);
	public updateResultsWithoutTotalCount = (): void => this.updateResults(false);
}
