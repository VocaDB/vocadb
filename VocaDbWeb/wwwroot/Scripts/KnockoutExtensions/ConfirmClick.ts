interface KnockoutBindingHandlers {
  confirmClick: KnockoutBindingHandler;
}

interface ConfirmClickData {
  message: string;
  click: () => void;
}

// From http://stackoverflow.com/a/24806534
ko.bindingHandlers.confirmClick = {
  init: (
    element: HTMLElement,
    valueAccessor: () => ConfirmClickData,
    viewModel,
  ) => {
    var value = valueAccessor();
    var message: string = ko.unwrap(value.message);
    var click = value.click;
    ko.applyBindingsToNode(
      element,
      {
        click: function () {
          if (confirm(message))
            return click.apply(this, Array.prototype.slice.apply(arguments));
        },
      },
      viewModel,
    );
  },
};
