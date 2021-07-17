import { functions } from '@/Shared/GlobalFunctions';
import { makeObservable, observable } from 'mobx';

interface ViewAuditLogContract {
	excludeUsers: string;
	filter: string;
	onlyNewUsers: boolean;
	userName: string;
}

export default class ViewAuditLogStore {
	@observable public excludeUsers = '';
	@observable public filter = '';
	@observable public filterVisible = false;
	@observable public onlyNewUsers = false;
	@observable public userName = '';

	public constructor(data: ViewAuditLogContract) {
		makeObservable(this);

		this.excludeUsers = data.excludeUsers;
		this.filter = data.filter;
		this.onlyNewUsers = data.onlyNewUsers;
		this.userName = data.userName;
		this.filterVisible =
			!functions.isNullOrWhiteSpace(data.userName) ||
			!functions.isNullOrWhiteSpace(data.excludeUsers) ||
			!functions.isNullOrWhiteSpace(data.filter) ||
			data.onlyNewUsers;

		// TODO
	}

	public toggleFilter = (): void => {
		this.filterVisible = !this.filterVisible;
	};

	private split(val: string): string[] {
		return val.split(/,\s*/);
	}

	private extractLast(term: string): string | undefined {
		return this.split(term).pop();
	}
}
