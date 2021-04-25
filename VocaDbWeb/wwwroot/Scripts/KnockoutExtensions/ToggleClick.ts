interface KnockoutBindingHandlers {
  // Toggles value of boolean observable
  toggleClick: KnockoutBindingHandler;
}

ko.bindingHandlers.toggleClick = {
  init: (
    element: HTMLElement,
    valueAccessor: () => KnockoutObservable<boolean>,
  ) => {
    var value = valueAccessor();

    ko.utils.registerEventHandler(element, 'click', () => {
      value(!value());
      return false;
    });
  },
};
