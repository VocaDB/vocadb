interface KnockoutBindingHandlers {
  // Gets the initial binding value from the element value instead of the model itself.
  // From http://www.tysoncadenhead.com/blog/using-knockout-for-progressive-enhancement
  initialValue: KnockoutBindingHandler;
}

ko.bindingHandlers.initialValue = {
  init: (element: HTMLElement, valueAccessor, allBindings): void => {
    var bindings = allBindings!();
    var val = $(element).val();
    if (bindings.value) {
      bindings.value(val);
    } else if (bindings.textInput) {
      bindings.textInput(val);
    } else if (bindings.checked) {
      bindings.checked((element as HTMLInputElement).checked);
    }
  },
};
