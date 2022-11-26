import { AuditLogEntryContract } from '@/DataContracts/AuditLogEntryContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { UserGroup } from '@/Models/Users/UserGroup';
import { AdminRepository } from '@/Repositories/AdminRepository';
import { PagedItemsStore } from '@/Stores/PagedItemsStore';
import { LocationStateStore, StateChangeEvent } from '@vocadb/route-sphere';
import Ajv from 'ajv';
import { computed, makeObservable, observable } from 'mobx';

import schema from './ViewAuditLogRouteParams.schema.json';

export interface ViewAuditLogRouteParams {
	excludeUsers?: string;
	filter?: string;
	group?: UserGroup;
	onlyNewUsers?: boolean;
	userName?: string;
}

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const validate = ajv.compile<ViewAuditLogRouteParams>(schema);

export class ViewAuditLogStore
	extends PagedItemsStore<AuditLogEntryContract>
	implements LocationStateStore<ViewAuditLogRouteParams> {
	@observable excludeUsers = '';
	@observable filter = '';
	@observable filterVisible = false;
	@observable group = UserGroup.Nothing;
	@observable onlyNewUsers = false;
	@observable userName = '';

	constructor(private readonly adminRepo: AdminRepository) {
		super();

		makeObservable(this);
	}

	@computed.struct get locationState(): ViewAuditLogRouteParams {
		return {
			excludeUsers: this.excludeUsers,
			filter: this.filter,
			group: this.group,
			onlyNewUsers: this.onlyNewUsers,
			userName: this.userName,
		};
	}
	set locationState(value: ViewAuditLogRouteParams) {
		this.excludeUsers = value.excludeUsers ?? '';
		this.filter = value.filter ?? '';
		this.group = value.group ?? UserGroup.Nothing;
		this.onlyNewUsers = value.onlyNewUsers ?? false;
		this.userName = value.userName ?? '';
	}

	validateLocationState = (
		locationState: any,
	): locationState is ViewAuditLogRouteParams => {
		return validate(locationState);
	};

	loadMoreItems = async (): Promise<
		PartialFindResultContract<AuditLogEntryContract>
	> => {
		const logEntries = await this.adminRepo.getAuditLogEntries({
			excludeUsers: this.excludeUsers,
			filter: this.filter,
			groupId: this.group !== UserGroup.Nothing ? this.group : undefined,
			onlyNewUsers: this.onlyNewUsers,
			userName: this.userName,
			start: this.start,
		});

		return { items: logEntries, totalCount: Number.MAX_VALUE /* TODO */ };
	};

	onLocationStateChange = (
		event: StateChangeEvent<ViewAuditLogRouteParams>,
	): void => {
		this.clear();
	};

	toggleFilter = (): void => {
		this.filterVisible = !this.filterVisible;
	};

	split(val: string): string[] {
		return val.split(/,\s*/);
	}

	extractLast(term: string): string | undefined {
		return this.split(term).pop();
	}
}
