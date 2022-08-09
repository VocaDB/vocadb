import { AdvancedFilterType } from '@/Stores/Search/AdvancedSearchFilter';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';
import _ from 'lodash';
import { action, makeObservable, observable } from 'mobx';

export class AdvancedSearchFilters {
	@observable public filters: AdvancedSearchFilter[] = [];

	public constructor() {
		makeObservable(this);
	}

	@action public add = (
		filter: AdvancedFilterType,
		param: string,
		description?: string,
		negate?: boolean,
	): void => {
		this.filters.push({
			filterType: filter,
			param: param,
			description: description,
			negate: negate,
		});
	};

	public hasFilter = (
		filterType: AdvancedFilterType,
		param: string,
	): boolean => {
		const result = this.filters.some(
			(f) => f.filterType === filterType && f.param === param,
		);
		return result;
	};

	@action public remove = (filter: AdvancedSearchFilter): void => {
		_.pull(this.filters, filter);
	};
}
