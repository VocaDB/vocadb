interface KnockoutBindingHandlers {

	timeAgo: KnockoutBindingHandler;

}

ko.bindingHandlers.timeAgo = {
	update: (element: HTMLElement, valueAccessor: () => KnockoutObservable<Date>) => {

		var val: Date = ko.unwrap(valueAccessor());
		var formatted = moment(val).fromNow();

		$(element).text(formatted);

	}
}