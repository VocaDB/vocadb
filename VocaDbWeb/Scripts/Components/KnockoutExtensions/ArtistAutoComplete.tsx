import { ArtistAutoCompleteParams } from '@/Components/KnockoutExtensions/AutoCompleteParams';
import EntryAutoComplete, {
	EntryAutoCompleteParams,
} from '@/Components/KnockoutExtensions/EntryAutoComplete';
import ArtistContract from '@/DataContracts/Artist/ArtistContract';
import functions from '@/Shared/GlobalFunctions';
import $ from 'jquery';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface ArtistAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	properties: ArtistAutoCompleteParams;
}

const ArtistAutoComplete = ({
	properties,
	...props
}: ArtistAutoCompleteProps): React.ReactElement => {
	const { t } = useTranslation(['VocaDb.Model.Resources']);

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
	if (properties.extraQueryParams)
		$.extend(queryParams, properties.extraQueryParams);

	const params: EntryAutoCompleteParams<ArtistContract> = {
		acceptSelection: properties.acceptSelection!,
		createNewItem: properties.createNewItem,
		createOptionFirstRow: (item) =>
			`${item.name} (${t(
				`VocaDb.Model.Resources:ArtistTypeNames:${item.artistType}`,
			)})`,
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
