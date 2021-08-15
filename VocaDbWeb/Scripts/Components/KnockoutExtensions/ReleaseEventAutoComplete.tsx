import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import functions from '@Shared/GlobalFunctions';
import React from 'react';

import EntryAutoComplete, {
	EntryAutoCompleteParams,
} from './EntryAutoComplete';

interface ReleaseEventAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	onAcceptSelection: (entry: ReleaseEventContract) => void;
	createNewItem?: string;
}

const ReleaseEventAutoComplete = ({
	onAcceptSelection,
	createNewItem,
	...props
}: ReleaseEventAutoCompleteProps): React.ReactElement => {
	const queryParams = {
		nameMatchMode: 'Auto',
		lang: vdb.values.languagePreference,
		preferAccurateMatches: true,
		maxResults: 20,
		sort: 'Name',
	};

	const params: EntryAutoCompleteParams<ReleaseEventContract> = {
		acceptSelection: (id, term, itemType, item) => {
			onAcceptSelection(
				item || {
					id: id!,
					artists: [],
					name: term!,
					webLinks: [],
					category: 'Unspecified',
					defaultNameLanguage: 'Undefined',
				},
			);
		},
		createOptionFirstRow: (item) => item.name,
		createNewItem: createNewItem,
		extraQueryParams: queryParams,
	};

	return (
		<EntryAutoComplete
			searchUrl={functions.mapAbsoluteUrl('/api/releaseEvents')}
			params={params}
			{...props}
		/>
	);
};

export default ReleaseEventAutoComplete;
