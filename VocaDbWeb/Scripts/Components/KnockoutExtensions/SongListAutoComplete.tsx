import SongListContract from '@/DataContracts/Song/SongListContract';
import SongListFeaturedCategory from '@/Models/SongLists/SongListFeaturedCategory';
import functions from '@/Shared/GlobalFunctions';
import React from 'react';

import EntryAutoComplete, {
	EntryAutoCompleteParams,
} from './EntryAutoComplete';

interface SongListAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	onAcceptSelection: (entry: SongListContract) => void;
	createNewItem?: string;
	songListCategory: SongListFeaturedCategory;
}

const SongListAutoComplete = ({
	onAcceptSelection,
	createNewItem,
	songListCategory,
	...props
}: SongListAutoCompleteProps): React.ReactElement => {
	const queryParams = {
		nameMatchMode: 'Auto',
		lang: vdb.values.languagePreference,
		preferAccurateMatches: true,
		maxResults: 20,
		sort: 'Name',
		featuredCategory: songListCategory,
	};

	const params: EntryAutoCompleteParams<SongListContract> = {
		acceptSelection: (id, term, itemType, item) => {
			onAcceptSelection(
				item || {
					id: id!,
					name: term!,
					author: null!,
					description: null!,
					featuredCategory: null!,
					status: null!,
				},
			);
		},
		createOptionFirstRow: (item) => item.name,
		createNewItem: createNewItem,
		extraQueryParams: queryParams,
	};

	return (
		<EntryAutoComplete
			searchUrl={functions.mapAbsoluteUrl('/api/songLists/featured')}
			params={params}
			{...props}
		/>
	);
};

export default SongListAutoComplete;
