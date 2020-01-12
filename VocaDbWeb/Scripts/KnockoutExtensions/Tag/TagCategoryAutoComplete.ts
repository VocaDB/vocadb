
import functions from "../../Shared/GlobalFunctions";

declare global {
	interface KnockoutBindingHandlers {
		tagCategoryAutoComplete: KnockoutBindingHandler;
	}
}

// Tag category autocomplete search box.
ko.bindingHandlers.tagCategoryAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<string>, allBindingsAccessor: () => any) => {

		var url = functions.mapAbsoluteUrl("/api/tags/categoryNames");
		var clearValue: boolean = ko.unwrap(allBindingsAccessor().clearValue);

		$(element).autocomplete({
			source: (ui, callback: (result: string[]) => void) => $.getJSON(url, { query: ui.term }, callback),
			select: (event: Event, ui) => {

				var value = valueAccessor();
				value(ui.item.label);

				if (clearValue) {
					$(element).val("");
					return false;					
				} else {
					return true;
				}

			}
		});

	}
}