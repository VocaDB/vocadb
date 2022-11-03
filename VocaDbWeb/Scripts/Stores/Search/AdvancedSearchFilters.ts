import { AdvancedFilterType } from '@/Stores/Search/AdvancedSearchFilter';
import { AdvancedSearchFilter } from '@/Stores/Search/AdvancedSearchFilter';
import { pull } from 'lodash-es';
import { action, makeObservable, observable } from 'mobx';

export class AdvancedSearchFilters {
	@observable filters: AdvancedSearchFilter[] = [];

	constructor() {
		makeObservable(this);
	}

	@action add = (
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

	hasFilter = (filterType: AdvancedFilterType, param: string): boolean => {
		const result = this.filters.some(
			(f) => f.filterType === filterType && f.param === param,
		);
		return result;
	};

	@action remove = (filter: AdvancedSearchFilter): void => {
		pull(this.filters, filter);
	};
}
