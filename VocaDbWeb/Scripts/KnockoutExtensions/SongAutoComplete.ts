/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../Shared/EntryAutoComplete.ts" />
/// <reference path="AutoCompleteParams.ts" />

interface KnockoutBindingHandlers {
    songAutoComplete: KnockoutBindingHandler;
}

// Song autocomplete search box.
ko.bindingHandlers.songAutoComplete = {
    init: (element: HTMLElement, valueAccessor) => {

        var properties: vdb.knockoutExtensions.SongAutoCompleteParams = ko.utils.unwrapObservable(valueAccessor());

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
			nameMatchMode: 'Auto',
			lang: vdb.models.globalization.ContentLanguagePreference[vdb.values.languagePreference],
			preferAccurateMatches: true
		};
		if (properties.extraQueryParams)
			jQuery.extend(queryParams, properties.extraQueryParams);

        vdb.initEntrySearch(element, "Song", vdb.functions.mapAbsoluteUrl("/api/songs"),
            {
                acceptSelection: properties.acceptSelection,
                createNewItem: properties.createNewItem,
				createCustomItem: properties.createCustomItem,
				createOptionFirstRow: (item: vdb.dataContracts.SongContract) => (item.name + " (" + item.songType + ")"),
				createOptionSecondRow: (item: vdb.dataContracts.SongContract) => (item.artistString),
				extraQueryParams: queryParams,
                filter: filter,
                height: properties.height,
				termParamName: 'query',
				method: 'GET'
            });


    }
}