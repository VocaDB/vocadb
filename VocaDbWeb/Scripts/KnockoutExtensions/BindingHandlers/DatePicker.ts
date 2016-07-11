
interface KnockoutBindingHandlers {
	datepicker: KnockoutBindingHandler;
}

interface DatePickerOptions {
	dateFormat: string;
	value: KnockoutObservable<Date>;
}

// Parts from https://github.com/gvas/knockout-jqueryui/blob/master/src/datepicker.js
ko.bindingHandlers.datepicker = {
	init: (element: HTMLElement, valueAccessor: () => DatePickerOptions) => {

		var options = valueAccessor();
		var value = ko.utils.unwrapObservable(options.value);

		$(element).datepicker({ dateFormat: options.dateFormat });

		if (value) {
			$(element).datepicker('setDate', value);
		}

		if (ko.isObservable(options.value)) {
			var subscription = options.value.subscribe((newValue: Date) => {
				$(element).datepicker('setDate', newValue);
			});

			ko.utils.domNodeDisposal.addDisposeCallback(element, () => {
				subscription.dispose();
			});
		}

		var selectDate = (selectedText: string) => {
			if (selectedText) {
				var format = $(element).datepicker('option', 'dateFormat');
				var parsed = $.datepicker.parseDate(format, selectedText);
				var date = moment.utc([parsed.getFullYear(), parsed.getMonth(), parsed.getDate()]).toDate(); // Make sure the date is parsed as UTC as we don't want any timezones here. jQuery UI seems to always parse as local.
				options.value(date);					
			} else {
				options.value(null);
			}
		}

		if (ko.isWriteableObservable(options.value)) {
			var origOnSelect = $(element).datepicker('option', 'onSelect');
			$(element).datepicker('option', 'onSelect', function(selectedText: string) {

				selectDate(selectedText);

				if (typeof origOnSelect === 'function') {
					origOnSelect.apply(this, Array.prototype.slice.call(arguments));
				}
			});
			$(element).change(() => {
				var val = $(element).val();
				selectDate(val);
			});
		}

	}
}