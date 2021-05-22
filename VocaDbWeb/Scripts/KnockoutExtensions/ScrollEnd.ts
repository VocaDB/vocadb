import $ from 'jquery';

declare global {
  interface KnockoutBindingHandlers {
    // Fires an event when the end of a scrollable area has been reached.
    // Currently only vertical scrolling is supported.
    scrollEnd: KnockoutBindingHandler;
  }
}

ko.bindingHandlers.scrollEnd = {
  init: (element: HTMLElement, valueAccessor: () => () => void): void => {
    var valFunc = valueAccessor();

    $(element).scroll(() => {
      if (element.scrollHeight - element.scrollTop === element.clientHeight) {
        valFunc();
      }
    });
  },
};
