import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { NameMatchMode } from '@/Models/NameMatchMode';
import { UserGroup } from '@/Models/Users/UserGroup';
import {
	UserOptionalField,
	UserRepository,
} from '@/Repositories/UserRepository';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import {
	includesAny,
	StateChangeEvent,
	LocationStateStore,
} from '@vocadb/route-sphere';
import Ajv from 'ajv';
import { computed, makeObservable, observable, runInAction } from 'mobx';

import schema from './ListUsersRouteParams.schema.json';

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

const clearResultsByQueryKeys: (keyof ListUsersRouteParams)[] = [
	'disabledUsers',
	'groupId',
	'knowsLanguage',
	'onlyVerifiedArtists',
	'pageSize',
	'filter',
];

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<ListUsersRouteParams>(schema);

export class ListUsersStore
	implements LocationStateStore<ListUsersRouteParams> {
	@observable disabledUsers = false;
	@observable group = UserGroup.Nothing;
	@observable loading = false;
	@observable knowsLanguage = '';
	@observable onlyVerifiedArtists = false;
	@observable page: UserApiContract[] = []; // Current page of items
	readonly paging = new ServerSidePagingStore(20); // Paging view model
	@observable searchTerm = '';
	@observable sort = UserSortRule.RegisterDate;

	constructor(private readonly userRepo: UserRepository) {
		makeObservable(this);
	}

	@computed.struct get locationState(): ListUsersRouteParams {
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
	set locationState(value: ListUsersRouteParams) {
		this.disabledUsers = value.disabledUsers ?? false;
		this.searchTerm = value.filter ?? '';
		this.group = value.groupId ?? UserGroup.Nothing;
		this.knowsLanguage = value.knowsLanguage ?? '';
		this.onlyVerifiedArtists = value.onlyVerifiedArtists ?? false;
		this.paging.page = value.page ?? 1;
		this.paging.pageSize = value.pageSize ?? 20;
		this.sort = value.sort ?? UserSortRule.RegisterDate;
	}

	validateLocationState = (data: any): data is ListUsersRouteParams => {
		return validate(data);
	};

	private pauseNotifications = false;

	updateResults = async (clearResults: boolean): Promise<void> => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;

		const pagingProperties = this.paging.getPagingProperties(clearResults);
		const result = await this.userRepo.getList({
			paging: pagingProperties,
			query: this.searchTerm,
			sort: this.sort,
			groups: this.group,
			includeDisabled: this.disabledUsers,
			onlyVerified: this.onlyVerifiedArtists,
			knowsLanguage: this.knowsLanguage,
			nameMatchMode: NameMatchMode.Auto,
			fields: [UserOptionalField.MainPicture],
		});

		this.pauseNotifications = false;

		runInAction(() => {
			this.page = result.items;

			if (pagingProperties.getTotalCount)
				this.paging.totalItems = result.totalCount;
		});
	};

	updateResultsWithTotalCount = (): Promise<void> => {
		return this.updateResults(true);
	};

	updateResultsWithoutTotalCount = (): Promise<void> => {
		return this.updateResults(false);
	};

	onLocationStateChange = (
		event: StateChangeEvent<ListUsersRouteParams>,
	): void => {
		const clearResults = includesAny(clearResultsByQueryKeys, event.keys);

		if (!event.popState && clearResults) this.paging.goToFirstPage();

		this.updateResults(clearResults);
	};
}
