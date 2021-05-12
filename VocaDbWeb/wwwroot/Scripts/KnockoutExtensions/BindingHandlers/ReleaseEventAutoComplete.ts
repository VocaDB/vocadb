import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import { EntryAutoCompleteParams } from '@Shared/EntryAutoComplete';
import { initEntrySearch } from '@Shared/EntryAutoComplete';
import functions from '@Shared/GlobalFunctions';

declare global {
  interface KnockoutBindingHandlers {
    releaseEventAutoComplete: KnockoutBindingHandler;
  }
}

// Tag autocomplete search box.
ko.bindingHandlers.releaseEventAutoComplete = {
  init: (
    element: HTMLElement,
    valueAccessor: () => KnockoutObservable<ReleaseEventContract>,
    allBindingsAccessor?: () => any,
  ): void => {
    var queryParams = {
      nameMatchMode: 'Auto',
      lang: ContentLanguagePreference[vdb.values.languagePreference],
      preferAccurateMatches: true,
      maxResults: 20,
      sort: 'Name',
    };

    var params: EntryAutoCompleteParams<ReleaseEventContract> = {
      acceptSelection: (id, term, itemType, item) => {
        valueAccessor()(
          item || {
            id: id!,
            artists: [],
            name: term!,
            webLinks: [],
            category: 'Unspecified',
            defaultNameLanguage: 'Undefined',
          },
        );
      },
      createOptionFirstRow: (item) => item.name,
      createNewItem: allBindingsAccessor!().createNewItem,
      extraQueryParams: queryParams,
    };

    initEntrySearch(
      element,
      functions.mapAbsoluteUrl('/api/releaseEvents'),
      params,
    );
  },
};
