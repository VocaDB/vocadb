// eslint-disable-next-line @typescript-eslint/no-unused-vars
interface KnockoutBindingHandlers {
  // Toggles value of boolean observable
  toggleClick: KnockoutBindingHandler;
}

ko.bindingHandlers.toggleClick = {
  init: (
    element: HTMLElement,
    valueAccessor: () => KnockoutObservable<boolean>,
  ): void => {
    var value = valueAccessor();

    ko.utils.registerEventHandler(element, 'click', () => {
      value(!value());
      return false;
    });
  },
};
