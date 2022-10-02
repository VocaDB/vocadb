import { AlbumAutoCompleteParams } from '@/Components/KnockoutExtensions/AutoCompleteParams';
import {
	EntryAutoComplete,
	EntryAutoCompleteParams,
} from '@/Components/KnockoutExtensions/EntryAutoComplete';
import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { functions } from '@/Shared/GlobalFunctions';
import $ from 'jquery';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface AlbumAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	properties: AlbumAutoCompleteParams;
}

export const AlbumAutoComplete = ({
	properties,
	...props
}: AlbumAutoCompleteProps): React.ReactElement => {
	const { t } = useTranslation(['VocaDb.Model.Resources.Albums']);

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
		preferAccurateMatches: true,
		maxResults: 15,
	};
	if (properties.extraQueryParams)
		$.extend(queryParams, properties.extraQueryParams);

	const params: EntryAutoCompleteParams<AlbumContract> = {
		acceptSelection: properties.acceptSelection!,
		createNewItem: properties.createNewItem,
		createCustomItem: properties.createCustomItem,
		createOptionFirstRow: (item) =>
			`${item.name} (${t(
				`VocaDb.Model.Resources.Albums:DiscTypeNames:${item.discType}`,
			)})`,
		createOptionSecondRow: (item) => item.artistString,
		extraQueryParams: queryParams,
		filter: filter,
		termParamName: 'query',
	};

	return (
		<EntryAutoComplete
			searchUrl={functions.mapAbsoluteUrl('/api/albums')}
			params={params}
			{...props}
		/>
	);
};
