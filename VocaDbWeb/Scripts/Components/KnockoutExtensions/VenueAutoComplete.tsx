import {
	EntryAutoComplete,
	EntryAutoCompleteParams,
} from '@/Components/KnockoutExtensions/EntryAutoComplete';
import { VenueForApiContract } from '@/DataContracts/Venue/VenueForApiContract';
import { functions } from '@/Shared/GlobalFunctions';
import React from 'react';

interface VenueAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	onAcceptSelection: (entry: VenueForApiContract) => void;
}

export const VenueAutoComplete = ({
	onAcceptSelection,
	...props
}: VenueAutoCompleteProps): React.ReactElement => {
	const queryParams = {
		nameMatchMode: 'Auto',
		lang: vdb.values.languagePreference,
		preferAccurateMatches: true,
		maxResults: 20,
		sort: 'Name',
	};

	const params: EntryAutoCompleteParams<VenueForApiContract> = {
		acceptSelection: (id, term, itemType, item) => {
			onAcceptSelection(item!);
		},
		createNewItem: null!,
		createOptionFirstRow: (item) => item.name,
		extraQueryParams: queryParams,
		termParamName: 'query',
		singleRow: true,
	};

	return (
		<EntryAutoComplete
			searchUrl={functions.mapAbsoluteUrl('/api/venues')}
			params={params}
			{...props}
		/>
	);
};
