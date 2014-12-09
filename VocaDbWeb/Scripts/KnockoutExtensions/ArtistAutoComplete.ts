/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../Shared/EntryAutoComplete.ts" />
/// <reference path="AutoCompleteParams.ts" />

interface KnockoutBindingHandlers {
    artistAutoComplete: KnockoutBindingHandler;
}

// Artist autocomplete search box.
ko.bindingHandlers.artistAutoComplete = {
    init: (element: HTMLElement, valueAccessor) => {

        var properties: vdb.knockoutExtensions.ArtistAutoCompleteParams = ko.utils.unwrapObservable(valueAccessor());

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
			fields: 'AdditionalNames',
			preferAccurateMatches: true
		};
		if (properties.extraQueryParams)
			jQuery.extend(queryParams, properties.extraQueryParams);

		var params: vdb.EntryAutoCompleteParams<vdb.dataContracts.ArtistContract> = {
			acceptSelection: properties.acceptSelection,
			createNewItem: properties.createNewItem,
			createOptionFirstRow: (item) => (item.name + " (" + item.artistType + ")"),
			createOptionSecondRow: (item) => (item.additionalNames),
			extraQueryParams: queryParams,
			filter: filter,
			height: properties.height,
			termParamName: 'query',
			method: 'GET'
		};

        vdb.initEntrySearch(element, "Artist", vdb.functions.mapAbsoluteUrl("/api/artists"), params);

    }
}