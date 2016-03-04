
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
			fields: 'AdditionalNames',
			lang: vdb.models.globalization.ContentLanguagePreference[vdb.values.languagePreference],
			preferAccurateMatches: true,
			maxResults: 20,
			sort: 'Name',
			allowAliases: allowAliases
		};

		var params: vdb.EntryAutoCompleteParams<dc.TagBaseContract> = {
			acceptSelection: (id, term, itemType, item) => {
				valueAccessor()(item);
			},
			createNewItem: null,
			createOptionFirstRow: (item) => item.name,
			createOptionSecondRow: null,
			extraQueryParams: queryParams,
			filter: tagFilter,
			termParamName: 'query'
		};

		vdb.initEntrySearch(element, "Tag", vdb.functions.mapAbsoluteUrl("/api/tags"), params);

	}
}