interface KnockoutBindingHandlers {
  slideVisible: KnockoutBindingHandler;
}

ko.bindingHandlers.slideVisible = {
  init: function (element, valueAccessor): void {
    // Initially set the element to be instantly visible/hidden depending on the value
    var value = ko.utils.unwrapObservable(valueAccessor());
    if (value) $(element).show();
  },
  update: function (element, valueAccessor, allBindingsAccessor): void {
    // Whenever the value subsequently changes, slowly fade the element in or out
    var value = valueAccessor();
    ko.utils.unwrapObservable(value)
      ? $(element).slideDown('fast')
      : $(element).slideUp('fast', () =>
          allBindingsAccessor!().complete(ko.dataFor(element)),
        );
  },
};
