
interface KnockoutBindingHandlers {
	tagAutoComplete: KnockoutBindingHandler;
}

// Tag autocomplete search box.
ko.bindingHandlers.tagAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => any, allBindingsAccessor: () => any) => {

		var tagFilter: (any) => boolean = allBindingsAccessor().tagFilter;
		var clearValue: boolean = ko.unwrap(allBindingsAccessor().clearValue);
		var allowAliases: boolean = ko.unwrap(allBindingsAccessor().allowAliases);

		if (clearValue == null)
			clearValue = true;

		var queryParams = {
			nameMatchMode: 'Auto',
			fields: 'AdditionalNames,CategoryName',
			lang: vdb.models.globalization.ContentLanguagePreference[vdb.values.languagePreference],
			preferAccurateMatches: true,
			maxResults: 20,
			sort: 'Name',
			allowAliases: allowAliases,
			target: ko.unwrap(allBindingsAccessor().tagTarget) || undefined
		};

		var params: vdb.EntryAutoCompleteParams<dc.TagApiContract> = {
			acceptSelection: (id, term, itemType, item) => {
				valueAccessor()(item);
			},
			createNewItem: null,
			createOptionFirstRow: (item) => item.name,
			createOptionSecondRow: (item) => (item.categoryName ? "(" + item.categoryName + ")" : null),
			extraQueryParams: queryParams,
			filter: tagFilter,
			termParamName: 'query',
			singleRow: true
		};

		vdb.initEntrySearch(element, vdb.functions.mapAbsoluteUrl("/api/tags"), params);

	}
}