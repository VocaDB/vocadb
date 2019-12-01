
import { EntryAutoCompleteParams } from '../../Shared/EntryAutoComplete';
import { initEntrySearch } from '../../Shared/EntryAutoComplete';
import { mapAbsoluteUrl } from '../../Shared/GlobalFunctions';
import UserApiContract from '../../DataContracts/User/UserApiContract';

declare global {
	interface KnockoutBindingHandlers {
		userAutocomplete: KnockoutBindingHandler;
	}
}

//module vdb.knockoutExtensions.bindingHandlers {

	export function userAutocomplete(element: HTMLElement, valueAccessor: () => any) {

		const params: EntryAutoCompleteParams<UserApiContract> = {
			acceptSelection: (id, term, itemType, item) => {
				valueAccessor()(item);
			},
			createNewItem: null,
			createOptionFirstRow: (item) => item.name,
			extraQueryParams: {},
			termParamName: 'query',
			singleRow: true
		};

		initEntrySearch(element, mapAbsoluteUrl("/api/users"), params);

	}

//}

ko.bindingHandlers.userAutocomplete = {
	init: userAutocomplete
}