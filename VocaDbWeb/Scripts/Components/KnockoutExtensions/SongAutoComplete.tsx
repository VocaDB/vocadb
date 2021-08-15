import SongContract from '@DataContracts/Song/SongContract';
import SearchTextQueryHelper from '@Helpers/SearchTextQueryHelper';
import NameMatchMode from '@Models/NameMatchMode';
import { SongQueryParams } from '@Repositories/SongRepository';
import functions from '@Shared/GlobalFunctions';
import $ from 'jquery';
import React from 'react';

import { SongAutoCompleteParams } from './AutoCompleteParams';
import EntryAutoComplete, {
	EntryAutoCompleteParams,
} from './EntryAutoComplete';

interface SongAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	properties: SongAutoCompleteParams;
}

const SongAutoComplete = ({
	properties,
	...props
}: SongAutoCompleteProps): React.ReactElement => {
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
		nameMatchMode: NameMatchMode[NameMatchMode.Auto],
		lang: vdb.values.languagePreference,
		preferAccurateMatches: true,
	};
	if (properties.extraQueryParams)
		$.extend(queryParams, properties.extraQueryParams);

	return (
		<EntryAutoComplete
			searchUrl={functions.mapAbsoluteUrl('/api/songs')}
			params={
				{
					acceptSelection: properties.acceptSelection!,
					createNewItem: properties.createNewItem,
					createCustomItem: properties.createCustomItem,
					createOptionFirstRow: (item: SongContract) =>
						item.name + ' (' + item.songType + ')',
					createOptionSecondRow: (item: SongContract) => item.artistString,
					extraQueryParams: queryParams,
					filter: filter,
					termParamName: 'query',
					onQuery: (searchQueryParams: SongQueryParams, term: string) => {
						// Increase the number of results for wildcard queries
						searchQueryParams.maxResults = SearchTextQueryHelper.isWildcardQuery(
							term,
						)
							? 30
							: 15;
					},
				} as EntryAutoCompleteParams<SongContract>
			}
			{...props}
		/>
	);
};

export default SongAutoComplete;
