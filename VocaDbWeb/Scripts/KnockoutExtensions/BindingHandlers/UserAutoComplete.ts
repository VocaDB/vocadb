
interface KnockoutBindingHandlers {
	userAutocomplete: KnockoutBindingHandler;
}

//module vdb.knockoutExtensions.bindingHandlers {

	export function userAutocomplete(element: HTMLElement, valueAccessor: () => any) {

		const params: vdb.EntryAutoCompleteParams<dc.user.UserApiContract> = {
			acceptSelection: (id, term, itemType, item) => {
				valueAccessor()(item);
			},
			createNewItem: null,
			createOptionFirstRow: (item) => item.name,
			extraQueryParams: {},
			termParamName: 'query',
			singleRow: true
		};

		vdb.initEntrySearch(element, vdb.functions.mapAbsoluteUrl("/api/users"), params);

	}

//}

ko.bindingHandlers.userAutocomplete = {
	init: vdb.knockoutExtensions.bindingHandlers.userAutocomplete
}