
interface KnockoutBindingHandlers {
	tagCategoryAutoComplete: KnockoutBindingHandler;
}

// Tag category autocomplete search box.
ko.bindingHandlers.tagCategoryAutoComplete = {
	init: (element: HTMLElement, valueAccessor: () => any) => {

		$(element).autocomplete({
			source: vdb.functions.mapAbsoluteUrl("/tag/findCategories"),
			select: (event, ui) => {
				valueAccessor()(ui.item.label);
				$(element).val("");
				return false;
			}
		});

	}
}