
interface KnockoutBindingHandlers {
	releaseEventAutoComplete: KnockoutBindingHandler;
}

// Tag autocomplete search box.
ko.bindingHandlers.releaseEventAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => any, allBindingsAccessor: () => any) => {

		var queryParams = {
			nameMatchMode: 'Auto',
			lang: vdb.models.globalization.ContentLanguagePreference[vdb.values.languagePreference],
			preferAccurateMatches: true,
			maxResults: 20,
			sort: 'Name'
		};

		var params: vdb.EntryAutoCompleteParams<dc.ReleaseEventContract> = {
			acceptSelection: (id, term, itemType, item) => {
				valueAccessor()(item);
			},
			createOptionFirstRow: (item) => item.name,
			extraQueryParams: queryParams
		};

		vdb.initEntrySearch(element, vdb.functions.mapAbsoluteUrl("/api/releaseEvents"), params);

	}

}