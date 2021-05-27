import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import IEntryWithIdAndName from '@Models/IEntryWithIdAndName';
import { EntryAutoCompleteParams } from '@Shared/EntryAutoComplete';
import { initEntrySearch } from '@Shared/EntryAutoComplete';
import functions from '@Shared/GlobalFunctions';
import ko from 'knockout';

declare global {
  interface KnockoutBindingHandlers {
    releaseEventSeriesAutoComplete: KnockoutBindingHandler;
  }
}

// Release event series autocomplete search box.
ko.bindingHandlers.releaseEventSeriesAutoComplete = {
  init: (
    element: HTMLElement,
    valueAccessor: () => any,
    allBindingsAccessor?: () => any,
  ): void => {
    var seriesFilter: (any: any) => boolean = allBindingsAccessor!().tagFilter;
    var clearValue: boolean = ko.unwrap(allBindingsAccessor!().clearValue);

    if (clearValue == null) clearValue = true;

    var queryParams = {
      nameMatchMode: 'Auto',
      preferAccurateMatches: true,
      lang: ContentLanguagePreference[vdb.values.languagePreference],
      maxResults: 20,
      sort: 'Name',
    };

    var params: EntryAutoCompleteParams<IEntryWithIdAndName> = {
      acceptSelection: (id, term, itemType, item) => {
        valueAccessor()(item);
      },
      createNewItem: null!,
      createOptionFirstRow: (item) => item.name!,
      createOptionSecondRow: null!,
      extraQueryParams: queryParams,
      filter: seriesFilter,
      termParamName: 'query',
    };

    initEntrySearch(
      element,
      functions.mapAbsoluteUrl('/api/releaseEventSeries'),
      params,
    );
  },
};
