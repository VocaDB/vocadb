import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { EventCategory } from '@/Models/Events/EventCategory';
import { EntryAutoCompleteParams } from '@/Shared/EntryAutoComplete';
import { initEntrySearch } from '@/Shared/EntryAutoComplete';
import { functions } from '@/Shared/GlobalFunctions';
import ko, { Observable } from 'knockout';

declare global {
	interface KnockoutBindingHandlers {
		releaseEventAutoComplete: KnockoutBindingHandler;
	}
}

// Tag autocomplete search box.
ko.bindingHandlers.releaseEventAutoComplete = {
	init: (
		element: HTMLElement,
		valueAccessor: () => Observable<ReleaseEventContract>,
		allBindingsAccessor?: () => any,
	): void => {
		var queryParams = {
			nameMatchMode: 'Auto',
			lang: vdb.values.languagePreference,
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
						category: EventCategory.Unspecified,
						defaultNameLanguage: 'Undefined',
						status: null!,
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
