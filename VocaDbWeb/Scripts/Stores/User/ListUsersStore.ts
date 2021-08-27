import UserApiContract from '@DataContracts/User/UserApiContract';
import UserGroup from '@Models/Users/UserGroup';
import UserRepository from '@Repositories/UserRepository';
import IStoreWithRouteParams from '@Stores/IStoreWithRouteParams';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
import { computed, makeObservable, observable, runInAction } from 'mobx';

// Corresponds to the UserSortRule enum in C#.
export enum UserSortRule {
	RegisterDate = 'RegisterDate',
	Name = 'Name',
	Group = 'Group',
}

export interface ListUsersRouteParams {
	disabledUsers?: boolean;
	filter?: string;
	groupId?: UserGroup;
	knowsLanguage?: string;
	onlyVerifiedArtists?: boolean;
	page?: number;
	pageSize?: number;
	sort?: UserSortRule;
}

export default class ListUsersStore
	implements IStoreWithRouteParams<ListUsersRouteParams> {
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
	}

	@computed.struct public get routeParams(): ListUsersRouteParams {
		return {
			disabledUsers: this.disabledUsers,
			filter: this.searchTerm,
			groupId: this.group,
			knowsLanguage: this.knowsLanguage,
			onlyVerifiedArtists: this.onlyVerifiedArtists,
			page: this.paging.page,
			pageSize: this.paging.pageSize,
			sort: this.sort,
		};
	}
	public set routeParams(value: ListUsersRouteParams) {
		this.disabledUsers = value.disabledUsers ?? false;
		this.searchTerm = value.filter ?? '';
		this.group = value.groupId ?? UserGroup.Nothing;
		this.knowsLanguage = value.knowsLanguage ?? '';
		this.onlyVerifiedArtists = value.onlyVerifiedArtists ?? false;
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 20;
		this.sort = value.sort ?? UserSortRule.RegisterDate;
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
