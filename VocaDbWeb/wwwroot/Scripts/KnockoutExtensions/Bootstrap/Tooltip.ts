
interface KnockoutBindingHandlers {
	// Shows bootstrap tooltip
	tooltip: KnockoutBindingHandler;
}

ko.bindingHandlers.tooltip = {
    init: (element: HTMLElement, params: Function) => {
		var unwrapped = ko.unwrap(params());
		$(element).tooltip(unwrapped);
    }
};
