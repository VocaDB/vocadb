interface KnockoutBindingHandlers {
  // Knockout binding for qTip tooltip.
  qTip: KnockoutBindingHandler;
}

ko.bindingHandlers.qTip = {
  init: (
    element: Element,
    valueAccessor: () => KnockoutObservable<QTipProperties>,
  ) => {
    var params = ko.unwrap(valueAccessor()) || {
      style: { classes: 'tooltip-wider' },
    };

    $(element).qtip(params);
  },
};
