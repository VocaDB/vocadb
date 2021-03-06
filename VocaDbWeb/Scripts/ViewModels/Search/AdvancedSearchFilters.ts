import ko from 'knockout';
import _ from 'lodash';

import AdvancedSearchFilter from './AdvancedSearchFilter';

export default class AdvancedSearchFilters {
	public add = (
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

	public filters = ko.observableArray<AdvancedSearchFilter>();

	public hasFilter = (filterType: string, param: string): boolean => {
		var result = _.some(
			this.filters(),
			(f) => f.filterType === filterType && f.param === param,
		);
		return result;
	};
}
