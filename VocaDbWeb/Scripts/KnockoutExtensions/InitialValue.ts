interface KnockoutBindingHandlers {
	// Gets the initial binding value from the element value instead of the model itself.
	// From http://www.tysoncadenhead.com/blog/using-knockout-for-progressive-enhancement
	initialValue: KnockoutBindingHandler;
}

ko.bindingHandlers.initialValue = {
	init: (element: HTMLElement, valueAccessor, allBindings) => {
		var bindings = allBindings();
		if (bindings.value) {
			bindings.value($(element).val());
		}
	}
}; 