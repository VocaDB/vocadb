
interface KnockoutBindingHandlers {
	venueAutoComplete: KnockoutBindingHandler;
}

// Venue autocomplete search box.
ko.bindingHandlers.venueAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<dc.VenueForApiContract>, allBindingsAccessor: () => any) => {

		var queryParams = {
			nameMatchMode: 'Auto',
			lang: vdb.models.globalization.ContentLanguagePreference[vdb.values.languagePreference],
			preferAccurateMatches: true,
			maxResults: 20,
			sort: 'Name'
		};

		const params: vdb.EntryAutoCompleteParams<dc.VenueForApiContract> = {
			acceptSelection: (id, term, itemType, item) => {
				valueAccessor()(item);
			},
			createNewItem: null,
			createOptionFirstRow: (item) => item.name,
			extraQueryParams: queryParams,
			termParamName: 'query',
			singleRow: true
		};

		vdb.initEntrySearch(element, vdb.functions.mapAbsoluteUrl("/api/venues"), params);

	}
}