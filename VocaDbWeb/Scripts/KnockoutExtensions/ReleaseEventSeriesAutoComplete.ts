
import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import { EntryAutoCompleteParams } from '../Shared/EntryAutoComplete';
import IEntryWithIdAndName from '../Models/IEntryWithIdAndName';
import { initEntrySearch } from '../Shared/EntryAutoComplete';
import { languagePreference } from '../Shared/GlobalValues';
import { mapAbsoluteUrl } from '../Shared/GlobalFunctions';

declare global {
	interface KnockoutBindingHandlers {
		releaseEventSeriesAutoComplete: KnockoutBindingHandler;
	}
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
			lang: ContentLanguagePreference[languagePreference],
			maxResults: 20,
			sort: 'Name'
		};

		var params: EntryAutoCompleteParams<IEntryWithIdAndName> = {
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

		initEntrySearch(element, mapAbsoluteUrl("/api/releaseEventSeries"), params);

	}
}