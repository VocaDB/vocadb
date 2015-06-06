 
interface KnockoutBindingHandlers {
	usernameAutocomplete: KnockoutBindingHandler;
}

module vdb.knockoutExtensions.bindingHandlers {

	export function usernameAutocomplete(element: HTMLElement, valueAccessor: () => any) {

		$(element).autocomplete({
			source: (ui, callback) => {
				var url = vdb.functions.mapAbsoluteUrl("/api/users/names");
				$.getJSON(url, { query: ui.term }, callback);
			},
			select: (event: Event, ui) => {
				
				var val = valueAccessor();

				if (val && ko.isWriteableObservable(val)) {
					val(ui.item.value);
					return false;
				}

				return true;

			}
		});

	}

}

ko.bindingHandlers.usernameAutocomplete = {
	init: vdb.knockoutExtensions.bindingHandlers.usernameAutocomplete
}