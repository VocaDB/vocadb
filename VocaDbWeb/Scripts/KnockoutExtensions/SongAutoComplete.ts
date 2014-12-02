/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../Shared/EntrySearchDrop.d.ts" />
/// <reference path="AutoCompleteParams.ts" />

interface KnockoutBindingHandlers {
    songAutoComplete: KnockoutBindingHandler;
}

// Song autocomplete search box.
ko.bindingHandlers.songAutoComplete = {
    init: function (element, valueAccessor) {

        var properties: vdb.knockoutExtensions.AutoCompleteParams = ko.utils.unwrapObservable(valueAccessor());

        initEntrySearch(element, null, "Song", vdb.functions.mapAbsoluteUrl("/Song/FindJsonByName"),
            {
                allowCreateNew: properties.allowCreateNew,
                acceptSelection: properties.acceptSelection,
                createNewItem: properties.createNewItem,
				createCustomItem: properties.createCustomItem,
                createOptionFirstRow: function (item) { return item.Name + " (" + item.SongType + ")"; },
                createOptionSecondRow: function (item) { return item.ArtistString; },
                extraQueryParams: properties.extraQueryParams,
                filter: properties.filter,
                height: properties.height
            });


    }
}