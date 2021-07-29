import TagApiContract from '@DataContracts/Tag/TagApiContract';
import functions from '@Shared/GlobalFunctions';
import React from 'react';

import EntryAutoComplete, {
	EntryAutoCompleteParams,
} from './EntryAutoComplete';

interface TagAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	onAcceptSelection: (entry: TagApiContract) => void;
	tagFilter?: (entry: TagApiContract) => boolean;
	clearValue?: boolean;
	allowAliases?: boolean;
	tagTarget?: any /* TODO */;
}

const TagAutoComplete = ({
	onAcceptSelection,
	tagFilter,
	clearValue = true,
	allowAliases,
	tagTarget,
	...props
}: TagAutoCompleteProps): React.ReactElement => {
	const queryParams = {
		nameMatchMode: 'Auto',
		fields: 'AdditionalNames',
		lang: vdb.values.languagePreference,
		preferAccurateMatches: true,
		maxResults: 20,
		sort: 'Name',
		allowAliases: allowAliases,
		target: tagTarget,
	};

	const params: EntryAutoCompleteParams<TagApiContract> = {
		acceptSelection: (id, term, itemType, item) => {
			onAcceptSelection(item!);
		},
		createOptionFirstRow: (item) => item.name,
		createOptionSecondRow: (item) =>
			item.categoryName ? '(' + item.categoryName + ')' : '',
		extraQueryParams: queryParams,
		filter: tagFilter,
		termParamName: 'query',
		singleRow: true,
	};

	return (
		<EntryAutoComplete
			searchUrl={functions.mapAbsoluteUrl('/api/tags')}
			params={params}
			{...props}
		/>
	);
};

export default TagAutoComplete;
