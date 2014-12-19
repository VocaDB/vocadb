/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../Shared/EntryAutoComplete.ts" />
/// <reference path="AutoCompleteParams.ts" />

interface KnockoutBindingHandlers {
	// Song autocomplete search box.
    songAutoComplete: KnockoutBindingHandler;
}

module vdb.knockoutExtensions {

	import cls = vdb.models;
	import dc = vdb.dataContracts;

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
			nameMatchMode: cls.NameMatchMode[cls.NameMatchMode.Auto],
			lang: cls.globalization.ContentLanguagePreference[vdb.values.languagePreference],
			preferAccurateMatches: true,
			maxResults: 15
		};
		if (properties.extraQueryParams)
			jQuery.extend(queryParams, properties.extraQueryParams);

		vdb.initEntrySearch(element, "Song", vdb.functions.mapAbsoluteUrl("/api/songs"),
			{
				acceptSelection: properties.acceptSelection,
				createNewItem: properties.createNewItem,
				createCustomItem: properties.createCustomItem,
				createOptionFirstRow: (item: dc.SongContract) => (item.name + " (" + item.songType + ")"),
				createOptionSecondRow: (item: dc.SongContract) => (item.artistString),
				extraQueryParams: queryParams,
				filter: filter,
				height: properties.height,
				termParamName: 'query',
				method: 'GET'
			});

	}

}

ko.bindingHandlers.songAutoComplete = {
	init: vdb.knockoutExtensions.songAutoComplete
}