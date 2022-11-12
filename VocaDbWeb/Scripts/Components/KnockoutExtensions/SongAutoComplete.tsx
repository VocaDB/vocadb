import { SongAutoCompleteParams } from '@/Components/KnockoutExtensions/AutoCompleteParams';
import {
	EntryAutoComplete,
	EntryAutoCompleteParams,
} from '@/Components/KnockoutExtensions/EntryAutoComplete';
import { SongContract } from '@/DataContracts/Song/SongContract';
import { SearchTextQueryHelper } from '@/Helpers/SearchTextQueryHelper';
import { NameMatchMode } from '@/Models/NameMatchMode';
import { SongQueryParams } from '@/Repositories/SongRepository';
import { functions } from '@/Shared/GlobalFunctions';
import { SongSortRule } from '@/Stores/Search/SongSearchStore';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface SongAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	properties: SongAutoCompleteParams;
}

export const SongAutoComplete = ({
	properties,
	...props
}: SongAutoCompleteProps): React.ReactElement => {
	const { t } = useTranslation(['VocaDb.Model.Resources.Songs']);

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
		sort: SongSortRule.SongType,
		...properties.extraQueryParams,
	};

	return (
		<EntryAutoComplete
			searchUrl={functions.mapAbsoluteUrl('/api/songs')}
			params={
				{
					acceptSelection: properties.acceptSelection!,
					createNewItem: properties.createNewItem,
					createCustomItem: properties.createCustomItem,
					createOptionFirstRow: (item: SongContract) =>
						`${item.name} (${t(
							`VocaDb.Model.Resources.Songs:SongTypeNames.${item.songType}`,
						)})`,
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
