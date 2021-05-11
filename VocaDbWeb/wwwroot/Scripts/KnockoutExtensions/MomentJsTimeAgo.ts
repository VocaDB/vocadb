// eslint-disable-next-line @typescript-eslint/no-unused-vars
interface KnockoutBindingHandlers {
  timeAgo: KnockoutBindingHandler;
}

ko.bindingHandlers.timeAgo = {
  update: (
    element: HTMLElement,
    valueAccessor: () => KnockoutObservable<Date>,
  ): void => {
    var val: Date = ko.unwrap(valueAccessor());
    var parsed = moment(val);

    $(element).text(parsed.fromNow());
    $(element).attr('title', parsed.format('l LT ([UTC]Z)')); // Short date and time with timezone
  },
};
