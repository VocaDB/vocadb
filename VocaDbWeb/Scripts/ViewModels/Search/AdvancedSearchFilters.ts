import AdvancedSearchFilter from '@/ViewModels/Search/AdvancedSearchFilter';
import ko from 'knockout';

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
		var result = this.filters().some(
			(f) => f.filterType === filterType && f.param === param,
		);
		return result;
	};
}
