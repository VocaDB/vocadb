/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../Shared/EntryAutoComplete.ts" />
/// <reference path="AutoCompleteParams.ts" />

import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import { initEntrySearch } from '../Shared/EntryAutoComplete';
import NameMatchMode from '../Models/NameMatchMode';
import SearchTextQueryHelper from '../Helpers/SearchTextQueryHelper';
import { SongAutoCompleteParams } from '../KnockoutExtensions/AutoCompleteParams';
import SongContract from '../DataContracts/Song/SongContract';
import { SongQueryParams } from '../Repositories/SongRepository';

declare global {
	interface KnockoutBindingHandlers {
		// Song autocomplete search box.
		songAutoComplete: KnockoutBindingHandler;
	}
}

//module vdb.knockoutExtensions {

	export function songAutoComplete(element: HTMLElement, valueAccessor) {
		
		var properties: SongAutoCompleteParams = ko.utils.unwrapObservable(valueAccessor());

		var filter = properties.filter;

		if (properties.ignoreId) {

			filter = (item) => {

				if (item.id == properties.ignoreId) {
					return false;
				}

				return properties.filter != null ? properties.filter(item) : true;

			}

		}

		var queryParams = {
			nameMatchMode: NameMatchMode[NameMatchMode.Auto],
			lang: ContentLanguagePreference[vdb.values.languagePreference],
			preferAccurateMatches: true
		};
		if (properties.extraQueryParams)
			jQuery.extend(queryParams, properties.extraQueryParams);

		initEntrySearch(element, vdb.functions.mapAbsoluteUrl("/api/songs"),
			{
				acceptSelection: properties.acceptSelection,
				createNewItem: properties.createNewItem,
				createCustomItem: properties.createCustomItem,
				createOptionFirstRow: (item: SongContract) => (item.name + " (" + item.songType + ")"),
				createOptionSecondRow: (item: SongContract) => (item.artistString),
				extraQueryParams: queryParams,
				filter: filter,
				termParamName: 'query',
				onQuery: (searchQueryParams: SongQueryParams, term: string) => {
					// Increase the number of results for wildcard queries
					searchQueryParams.maxResults = SearchTextQueryHelper.isWildcardQuery(term) ? 30 : 15;
				}
			});

	}

//}

ko.bindingHandlers.songAutoComplete = {
	init: songAutoComplete
}