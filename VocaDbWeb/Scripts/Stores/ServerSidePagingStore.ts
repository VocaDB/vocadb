import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { range } from 'lodash-es';
import { action, computed, makeObservable, observable } from 'mobx';

export class ServerSidePagingStore {
	@observable page = 1;
	@observable totalItems = 0;
	@observable pageSize = 10;

	constructor(pageSize: number = 10) {
		makeObservable(this);

		this.pageSize = pageSize;
	}

	@computed get firstItem(): number {
		return (this.page - 1) * this.pageSize;
	}

	@computed get totalPages(): number {
		return Math.ceil(this.totalItems / this.pageSize);
	}

	@computed get hasMultiplePages(): boolean {
		return this.totalPages > 1;
	}

	@computed get isFirstPage(): boolean {
		return this.page <= 1;
	}

	@computed get isLastPage(): boolean {
		return this.page >= this.totalPages;
	}

	@computed get pages(): number[] {
		const start = Math.max(this.page - 4, 1);
		const end = Math.min(this.page + 4, this.totalPages);

		return end >= start ? range(start, end + 1) : [];
	}

	@computed get showMoreBegin(): boolean {
		return this.page > 5;
	}

	@computed get showMoreEnd(): boolean {
		return this.page < this.totalPages - 4;
	}

	getPagingProperties = (clearResults: boolean = false): PagingProperties => {
		return {
			start: this.firstItem,
			maxEntries: this.pageSize,
			getTotalCount: clearResults || this.totalItems === 0,
		};
	};

	@action goToFirstPage = (): void => {
		this.page = 1;
	};

	@action goToLastPage = (): void => {
		this.page = this.totalPages;
	};

	@action nextPage = (): void => {
		if (!this.isLastPage) this.page = this.page + 1;
	};

	@action previousPage = (): void => {
		if (!this.isFirstPage) this.page = this.page - 1;
	};
}
