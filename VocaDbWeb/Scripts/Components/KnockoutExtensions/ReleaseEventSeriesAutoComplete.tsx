import {
	EntryAutoComplete,
	EntryAutoCompleteParams,
} from '@/Components/KnockoutExtensions/EntryAutoComplete';
import { IEntryWithIdAndName } from '@/Models/IEntryWithIdAndName';
import { functions } from '@/Shared/GlobalFunctions';
import React from 'react';

interface ReleaseEventSeriesAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	onAcceptSelection: (entry: IEntryWithIdAndName) => void;
	seriesFilter?: (entry: IEntryWithIdAndName) => boolean;
}

export const ReleaseEventSeriesAutoComplete = ({
	onAcceptSelection,
	seriesFilter,
	...props
}: ReleaseEventSeriesAutoCompleteProps): React.ReactElement => {
	const queryParams = {
		nameMatchMode: 'Auto',
		preferAccurateMatches: true,
		lang: vdb.values.languagePreference,
		maxResults: 20,
		sort: 'Name',
	};

	const params: EntryAutoCompleteParams<IEntryWithIdAndName> = {
		acceptSelection: (id, term, itemType, item) => {
			onAcceptSelection(item!);
		},
		createNewItem: null!,
		createOptionFirstRow: (item) => item.name!,
		createOptionSecondRow: null!,
		extraQueryParams: queryParams,
		filter: seriesFilter,
		termParamName: 'query',
	};

	return (
		<EntryAutoComplete
			searchUrl={functions.mapAbsoluteUrl('/api/releaseEventSeries')}
			params={params}
			{...props}
		/>
	);
};
