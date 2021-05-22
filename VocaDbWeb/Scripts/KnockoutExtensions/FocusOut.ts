import $ from 'jquery';

declare global {
  interface KnockoutBindingHandlers {
    // Binding handler for jQuery focusout event.
    focusout: KnockoutBindingHandler;
  }
}

ko.bindingHandlers.focusout = {
  init: function (element, valueAccessor): void {
    var value: any = ko.utils.unwrapObservable(valueAccessor());
    $(element).focusout(value);
  },
};
