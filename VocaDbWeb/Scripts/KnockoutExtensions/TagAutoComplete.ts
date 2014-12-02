
interface KnockoutBindingHandlers {
	tagAutoComplete: KnockoutBindingHandler;
}

// Tag autocomplete search box.
ko.bindingHandlers.tagAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => any, allBindingsAccessor: () => any) => {

		var tagFilter: (string) => boolean = allBindingsAccessor().tagFilter;
		var clearValue: boolean = ko.unwrap(allBindingsAccessor().clearValue);

		if (clearValue == null)
			clearValue = true;

		$(element).autocomplete({
			source: (ui, callback) => {
				$.getJSON(vdb.functions.mapAbsoluteUrl("/api/tags/names"), { query: ui.term }, (result: string[]) => {
					var tags = tagFilter ? _.filter(result, tagFilter) : result;
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