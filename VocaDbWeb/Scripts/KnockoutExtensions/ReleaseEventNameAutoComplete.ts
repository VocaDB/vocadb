
interface KnockoutBindingHandlers {
	releaseEventNameAutoComplete: KnockoutBindingHandler;
}

// Tag autocomplete search box.
ko.bindingHandlers.releaseEventNameAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => any, allBindingsAccessor: () => any) => {

		var releaseEventFilter: (string) => boolean = allBindingsAccessor().tagFilter;
		var clearValue: boolean = ko.unwrap(allBindingsAccessor().clearValue);

		if (clearValue == null)
			clearValue = true;

		$(element).autocomplete({
			source: (ui, callback) => {
				$.getJSON(vdb.functions.mapAbsoluteUrl("/api/releaseEvents/names"), { query: ui.term }, (result: string[]) => {
					var tags = releaseEventFilter ? _.filter(result, releaseEventFilter) : result;
					callback(tags);
				});
			},
			select: (event, ui) => {
				valueAccessor()(ui.item.label);
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