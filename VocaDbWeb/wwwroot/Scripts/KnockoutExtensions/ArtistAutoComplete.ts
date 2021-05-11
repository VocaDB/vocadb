import { ArtistAutoCompleteParams } from '@KnockoutExtensions/AutoCompleteParams';
import ArtistContract from '@DataContracts/Artist/ArtistContract';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import { EntryAutoCompleteParams } from '@Shared/EntryAutoComplete';
import functions from '@Shared/GlobalFunctions';
import { initEntrySearch } from '@Shared/EntryAutoComplete';

declare global {
  interface KnockoutBindingHandlers {
    // Artist autocomplete search box.
    artistAutoComplete: KnockoutBindingHandler;
  }
}

export function artistAutoComplete(
  element: HTMLElement,
  valueAccessor: () => any,
): void {
  var properties: ArtistAutoCompleteParams = ko.utils.unwrapObservable(
    valueAccessor(),
  );

  var filter = properties.filter;

  if (properties.ignoreId) {
    filter = (item): boolean => {
      if (item.id === properties.ignoreId) {
        return false;
      }

      return properties.filter != null ? properties.filter(item) : true;
    };
  }

  var queryParams = {
    nameMatchMode: 'Auto',
    lang: ContentLanguagePreference[vdb.values.languagePreference],
    fields: 'AdditionalNames',
    preferAccurateMatches: true,
    maxResults: 20,
  };
  if (properties.extraQueryParams)
    jQuery.extend(queryParams, properties.extraQueryParams);

  var params: EntryAutoCompleteParams<ArtistContract> = {
    acceptSelection: properties.acceptSelection!,
    createNewItem: properties.createNewItem,
    createOptionFirstRow: (item) => item.name + ' (' + item.artistType + ')',
    createOptionSecondRow: (item) => item.additionalNames!,
    extraQueryParams: queryParams,
    filter: filter,
    termParamName: 'query',
  };

  initEntrySearch(element, functions.mapAbsoluteUrl('/api/artists'), params);
}

ko.bindingHandlers.artistAutoComplete = {
  init: artistAutoComplete,
};
