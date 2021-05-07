interface KnockoutBindingHandlers {
  // Shows the element when the model has been bound.
  // Can be used in conjunction with the js-cloak class.
  show: KnockoutBindingHandler;
}

ko.bindingHandlers.show = {
  init: (element: HTMLElement): void => {
    $(element).css('visibility', 'visible');
  },
};
