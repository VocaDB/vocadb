
interface KnockoutBindingHandlers {
	songListAutoComplete: KnockoutBindingHandler;
}

// Tag autocomplete search box.
ko.bindingHandlers.songListAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<dc.SongListContract>, allBindingsAccessor: () => any) => {

		var allBindings = allBindingsAccessor();
		var category: string = allBindings.songListCategory;

		var queryParams = {
			nameMatchMode: 'Auto',
			lang: vdb.models.globalization.ContentLanguagePreference[vdb.values.languagePreference],
			preferAccurateMatches: true,
			maxResults: 20,
			sort: 'Name',
			featuredCategory: category
		};	

		var params: vdb.EntryAutoCompleteParams<dc.SongListContract> = {
			acceptSelection: (id, term, itemType, item) => {
				valueAccessor()(item || { id: id, name: term, author: null, description: null, featuredCategory: null, status: null });
			},
			createOptionFirstRow: (item) => item.name,
			createNewItem: allBindingsAccessor().createNewItem,
			extraQueryParams: queryParams
		};

		vdb.initEntrySearch(element, vdb.functions.mapAbsoluteUrl("/api/songLists/featured"), params);

	}

}