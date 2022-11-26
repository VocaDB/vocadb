import {
	EntryAutoComplete,
	EntryAutoCompleteParams,
} from '@/Components/KnockoutExtensions/EntryAutoComplete';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { EventCategory } from '@/Models/Events/EventCategory';
import { functions } from '@/Shared/GlobalFunctions';
import { useVdb } from '@/VdbContext';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ReleaseEventAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	onAcceptSelection: (entry: ReleaseEventContract) => void;
}

export const ReleaseEventAutoComplete = ({
	onAcceptSelection,
	...props
}: ReleaseEventAutoCompleteProps): React.ReactElement => {
	const vdb = useVdb();

	const { t } = useTranslation(['VocaDb.Web.Resources.Domain.ReleaseEvents']);

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
					category: EventCategory.Unspecified,
					defaultNameLanguage: 'Undefined',
					status: null!,
				},
			);
		},
		createOptionFirstRow: (item) =>
			`${item.name} (${t(
				`VocaDb.Web.Resources.Domain.ReleaseEvents:EventCategoryNames.${item.category}`,
			)})`,
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
