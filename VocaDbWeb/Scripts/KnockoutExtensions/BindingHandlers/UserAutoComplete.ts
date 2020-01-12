
import { EntryAutoCompleteParams } from '../../Shared/EntryAutoComplete';
import functions from '../../Shared/GlobalFunctions';
import { initEntrySearch } from '../../Shared/EntryAutoComplete';
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

		initEntrySearch(element, functions.mapAbsoluteUrl("/api/users"), params);

	}

//}

ko.bindingHandlers.userAutocomplete = {
	init: userAutocomplete
}