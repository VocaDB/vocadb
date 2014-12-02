/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../Shared/EntrySearchDrop.d.ts" />
/// <reference path="AutoCompleteParams.ts" />

interface KnockoutBindingHandlers {
    artistAutoComplete: KnockoutBindingHandler;
}

// Artist autocomplete search box.
ko.bindingHandlers.artistAutoComplete = {
    init: function (element, valueAccessor) {

        var properties: vdb.knockoutExtensions.AutoCompleteParams = ko.utils.unwrapObservable(valueAccessor());

		var filter = properties.filter;

		if (properties.ignoreId) {

			filter = (item) => {

				if (properties.ignoreId && item.Id == properties.ignoreId) {
					return false;
				}

				return properties.filter != null ? properties.filter(item) : true;

			}

		}

        initEntrySearch(element, null, "Artist", vdb.functions.mapAbsoluteUrl("/Artist/FindJson"),
            {
                allowCreateNew: properties.allowCreateNew,
				acceptSelection: properties.acceptSelection,
				createNewItem: properties.createNewItem,
                createOptionFirstRow: function (item) { return item.Name + " (" + item.ArtistType + ")"; },
                createOptionSecondRow: function (item) { return item.AdditionalNames; },
                extraQueryParams: properties.extraQueryParams,
				filter: filter,
                height: properties.height
            });


    }
}