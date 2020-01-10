
import AdvancedSearchFilter from './AdvancedSearchFilter';

//namespace vdb.viewModels.search {

	export default class AdvancedSearchFilters {

		public add = (filter: string, param: string, description: string, negate?: boolean) => {
			this.filters.push({ filterType: filter, param: param, description: description, negate: negate });
		}

		public filters = ko.observableArray<AdvancedSearchFilter>();

		public hasFilter = (filterType: string, param: string) => {
			var result = _.some(this.filters(), f => f.filterType === filterType && f.param === param);
			return result;
		}

	}

//}