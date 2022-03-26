import Dropdown from '@Bootstrap/Dropdown';
import { AdvancedFilterType } from '@Stores/Search/AdvancedSearchFilter';
import AdvancedSearchFilters from '@Stores/Search/AdvancedSearchFilters';
import React from 'react';

interface AdvancedFilterProps {
	advancedFilters: AdvancedSearchFilters;
	description?: string;
	filterType: AdvancedFilterType;
	param: string;
	negate?: boolean;
}

const AdvancedFilter = ({
	advancedFilters,
	description,
	filterType,
	param,
	negate,
}: AdvancedFilterProps): React.ReactElement => {
	return advancedFilters.hasFilter(filterType, param) ? (
		<></>
	) : (
		<Dropdown.Item
			onClick={(): void =>
				advancedFilters.add(filterType, param, description, negate)
			}
		>
			{description}
		</Dropdown.Item>
	);
};

export default AdvancedFilter;
