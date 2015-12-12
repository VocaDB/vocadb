
interface KnockoutBindingHandlers {
	tooltip: KnockoutBindingHandler;
}

ko.bindingHandlers.tooltip = {
    init: (element: HTMLElement, params: any) => {
		$(element).tooltip(ko.unwrap(params));
    }
};
