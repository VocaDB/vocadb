import $ from 'jquery';

declare global {
  interface KnockoutBindingHandlers {
    // Shows bootstrap tooltip
    tooltip: KnockoutBindingHandler;
  }
}

ko.bindingHandlers.tooltip = {
  init: (element: HTMLElement, params: Function): void => {
    var unwrapped = ko.unwrap(params());
    $(element).tooltip(unwrapped);
  },
};
