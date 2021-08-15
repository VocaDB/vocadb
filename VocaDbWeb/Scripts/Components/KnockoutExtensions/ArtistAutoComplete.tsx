import ArtistContract from '@DataContracts/Artist/ArtistContract';
import functions from '@Shared/GlobalFunctions';
import React from 'react';

import { ArtistAutoCompleteParams } from './AutoCompleteParams';
import EntryAutoComplete, {
	EntryAutoCompleteParams,
} from './EntryAutoComplete';

interface ArtistAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	properties: ArtistAutoCompleteParams;
}

const ArtistAutoComplete = ({
	properties,
	...props
}: ArtistAutoCompleteProps): React.ReactElement => {
	var filter = properties.filter;

	if (properties.ignoreId) {
		filter = (item): boolean => {
			if (item.id === properties.ignoreId) {
				return false;
			}

			return properties.filter != null ? properties.filter(item) : true;
		};
	}

	const queryParams = {
		nameMatchMode: 'Auto',
		lang: vdb.values.languagePreference,
		fields: 'AdditionalNames',
		preferAccurateMatches: true,
		maxResults: 20,
	};

	const params: EntryAutoCompleteParams<ArtistContract> = {
		acceptSelection: properties.acceptSelection!,
		createNewItem: properties.createNewItem,
		createOptionFirstRow: (item) => `${item.name} (${item.artistType})`,
		createOptionSecondRow: (item) => item.additionalNames!,
		extraQueryParams: queryParams,
		filter: filter,
		termParamName: 'query',
	};

	return (
		<EntryAutoComplete
			searchUrl={functions.mapAbsoluteUrl('/api/artists')}
			params={params}
			{...props}
		/>
	);
};

export default ArtistAutoComplete;
