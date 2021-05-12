// eslint-disable-next-line @typescript-eslint/no-unused-vars
interface KnockoutBindingHandlers {
  // Shows bootstrap tooltip
  tooltip: KnockoutBindingHandler;
}

ko.bindingHandlers.tooltip = {
  init: (element: HTMLElement, params: Function): void => {
    var unwrapped = ko.unwrap(params());
    $(element).tooltip(unwrapped);
  },
};
