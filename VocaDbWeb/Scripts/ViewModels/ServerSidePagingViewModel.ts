import PagingProperties from '@DataContracts/PagingPropertiesContract';
import ko from 'knockout';
import _ from 'lodash';

export default class ServerSidePagingViewModel {
	public constructor(pageSize: number = 10) {
		this.pageSize(pageSize);
		this.page.subscribe(this.updateItems);
		this.pageSize.subscribe(this.updateItems);
	}

	public updateItems = (): void => {
		if (!this.getItemsCallback) return;

		this.getItemsCallback(this.getPagingProperties(false));
	};

	public getItemsCallback!: (paging: PagingProperties) => void;

	public page = ko.observable(1);

	public totalItems = ko.observable(0);

	public pageSize = ko.observable(10);

	public firstItem = ko.computed(() => (this.page() - 1) * this.pageSize());

	public totalPages = ko.computed(() =>
		Math.ceil(this.totalItems() / this.pageSize()),
	);

	public hasMultiplePages = ko.computed(() => this.totalPages() > 1);

	public isFirstPage = ko.computed(() => this.page() <= 1);

	public isLastPage = ko.computed(() => this.page() >= this.totalPages());

	public pages = ko.computed(() => {
		var start = Math.max(this.page() - 4, 1);
		var end = Math.min(this.page() + 4, this.totalPages());

		return _.range(start, end + 1);
	});

	public showMoreBegin = ko.computed(() => this.page() > 5);

	public showMoreEnd = ko.computed(() => this.page() < this.totalPages() - 4);

	public getPagingProperties = (
		clearResults: boolean = false,
	): { start: number; maxEntries: number; getTotalCount: boolean } => {
		return {
			start: this.firstItem(),
			maxEntries: this.pageSize(),
			getTotalCount: clearResults || this.totalItems() === 0,
		};
	};

	public goToFirstPage = (): void => this.page(1);

	public goToLastPage = (): void => this.page(this.totalPages());

	public nextPage = (): void => {
		if (!this.isLastPage()) this.page(this.page() + 1);
	};

	public previousPage = (): void => {
		if (!this.isFirstPage()) this.page(this.page() - 1);
	};
}
