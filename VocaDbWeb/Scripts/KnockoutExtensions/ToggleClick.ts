import ko, { Observable } from 'knockout';

declare global {
  interface KnockoutBindingHandlers {
    // Toggles value of boolean observable
    toggleClick: KnockoutBindingHandler;
  }
}

ko.bindingHandlers.toggleClick = {
  init: (
    element: HTMLElement,
    valueAccessor: () => Observable<boolean>,
  ): void => {
    var value = valueAccessor();

    ko.utils.registerEventHandler(element, 'click', () => {
      value(!value());
      return false;
    });
  },
};
