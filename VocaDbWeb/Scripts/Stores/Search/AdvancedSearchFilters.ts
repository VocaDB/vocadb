import _ from 'lodash';
import { action, makeObservable, observable } from 'mobx';

import AdvancedSearchFilter from './AdvancedSearchFilter';

export default class AdvancedSearchFilters {
	@observable public filters: AdvancedSearchFilter[] = [];

	public constructor() {
		makeObservable(this);
	}

	@action public add = (
		filter: string,
		param: string,
		description: string,
		negate?: boolean,
	): void => {
		this.filters.push({
			filterType: filter,
			param: param,
			description: description,
			negate: negate,
		});
	};

	public hasFilter = (filterType: string, param: string): boolean => {
		const result = _.some(
			this.filters,
			(f) => f.filterType === filterType && f.param === param,
		);
		return result;
	};
}
