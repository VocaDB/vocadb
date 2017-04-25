
interface KnockoutBindingHandlers {
	releaseEventAutoComplete: KnockoutBindingHandler;
}

// Tag autocomplete search box.
ko.bindingHandlers.releaseEventAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<dc.ReleaseEventContract>, allBindingsAccessor: () => any) => {

		var queryParams = {
			nameMatchMode: 'Auto',
			lang: vdb.models.globalization.ContentLanguagePreference[vdb.values.languagePreference],
			preferAccurateMatches: true,
			maxResults: 20,
			sort: 'Name'
		};	

		var params: vdb.EntryAutoCompleteParams<dc.ReleaseEventContract> = {
			acceptSelection: (id, term, itemType, item) => {
				valueAccessor()(item || { id: id, name: term, webLinks: [], defaultNameLanguage: 'Undefined' });
			},
			createOptionFirstRow: (item) => item.name,
			createNewItem: allBindingsAccessor().createNewItem,
			extraQueryParams: queryParams
		};

		vdb.initEntrySearch(element, vdb.functions.mapAbsoluteUrl("/api/releaseEvents"), params);

	}

}