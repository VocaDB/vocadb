
interface KnockoutBindingHandlers {
	releaseEventSeriesAutoComplete: KnockoutBindingHandler;
}

// Release event series autocomplete search box.
ko.bindingHandlers.releaseEventSeriesAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => any, allBindingsAccessor: () => any) => {

		var seriesFilter: (any) => boolean = allBindingsAccessor().tagFilter;
		var clearValue: boolean = ko.unwrap(allBindingsAccessor().clearValue);

		if (clearValue == null)
			clearValue = true;

		var queryParams = {
			nameMatchMode: 'Auto',
			preferAccurateMatches: true,
			maxResults: 20,
			sort: 'Name'
		};

		var params: vdb.EntryAutoCompleteParams<vdb.models.IEntryWithIdAndName> = {
			acceptSelection: (id, term, itemType, item) => {
				valueAccessor()(item);
			},
			createNewItem: null,
			createOptionFirstRow: (item) => item.name,
			createOptionSecondRow: null,
			extraQueryParams: queryParams,
			filter: seriesFilter,
			termParamName: 'query'
		};

		vdb.initEntrySearch(element, vdb.functions.mapAbsoluteUrl("/api/releaseEventSeries"), params);

	}
}