import $ from 'jquery';
import ko, { Observable } from 'knockout';

declare global {
  interface KnockoutBindingHandlers {
    // Knockout binding for qTip tooltip.
    qTip: KnockoutBindingHandler;
  }
}

ko.bindingHandlers.qTip = {
  init: (
    element: Element,
    valueAccessor: () => Observable<QTipProperties>,
  ): void => {
    var params = ko.unwrap(valueAccessor()) || {
      style: { classes: 'tooltip-wider' },
    };

    $(element).qtip(params);
  },
};
