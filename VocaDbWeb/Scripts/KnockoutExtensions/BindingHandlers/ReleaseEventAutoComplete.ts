
import ContentLanguagePreference from '../../Models/Globalization/ContentLanguagePreference';
import { EntryAutoCompleteParams } from '../../Shared/EntryAutoComplete';
import { initEntrySearch } from '../../Shared/EntryAutoComplete';
import { languagePreference } from '../../Shared/GlobalValues';
import { mapAbsoluteUrl } from '../../Shared/GlobalFunctions';
import ReleaseEventContract from '../../DataContracts/ReleaseEvents/ReleaseEventContract';

declare global {
	interface KnockoutBindingHandlers {
		releaseEventAutoComplete: KnockoutBindingHandler;
	}
}

// Tag autocomplete search box.
ko.bindingHandlers.releaseEventAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<ReleaseEventContract>, allBindingsAccessor: () => any) => {

		var queryParams = {
			nameMatchMode: 'Auto',
			lang: ContentLanguagePreference[languagePreference],
			preferAccurateMatches: true,
			maxResults: 20,
			sort: 'Name'
		};	

		var params: EntryAutoCompleteParams<ReleaseEventContract> = {
			acceptSelection: (id, term, itemType, item) => {
				valueAccessor()(item || { id: id, artists: [], name: term, webLinks: [], category: 'Unspecified', defaultNameLanguage: 'Undefined' });
			},
			createOptionFirstRow: (item) => item.name,
			createNewItem: allBindingsAccessor().createNewItem,
			extraQueryParams: queryParams
		};

		initEntrySearch(element, mapAbsoluteUrl("/api/releaseEvents"), params);

	}

}