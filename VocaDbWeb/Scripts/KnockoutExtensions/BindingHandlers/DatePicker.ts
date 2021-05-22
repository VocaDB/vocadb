import DateTimeHelper from '@Helpers/DateTimeHelper';
import $ from 'jquery';

declare global {
  interface KnockoutBindingHandlers {
    datepicker: KnockoutBindingHandler;
  }
}

interface DatePickerOptions {
  dateFormat: string;
  // Current date. This is assumed to be UTC/GMT (for example, in GMT+2, hours would be 2).
  value: KnockoutObservable<Date>;
}

// Parts from https://github.com/gvas/knockout-jqueryui/blob/master/src/datepicker.js
ko.bindingHandlers.datepicker = {
  init: (
    element: HTMLElement,
    valueAccessor: () => DatePickerOptions,
  ): void => {
    var options = valueAccessor();
    var value = ko.utils.unwrapObservable(options.value);

    $(element).datepicker({ dateFormat: options.dateFormat });

    if (value) {
      $(element).datepicker('setDate', DateTimeHelper.convertToLocal(value));
    }

    if (ko.isObservable(options.value)) {
      var subscription = options.value.subscribe((newValue: Date) => {
        // datepicker displays time in local time, so we convert it back to local
        $(element).datepicker(
          'setDate',
          DateTimeHelper.convertToLocal(newValue),
        );
      });

      ko.utils.domNodeDisposal.addDisposeCallback(element, () => {
        subscription.dispose();
      });
    }

    var selectDate = (selectedText: string): void => {
      if (selectedText) {
        const format = $(element).datepicker('option', 'dateFormat');
        const parsed = $.datepicker.parseDate(format, selectedText);
        // Make sure the date is parsed as UTC as we don't want any timezones here. jQuery UI seems to always parse as local.
        const date = DateTimeHelper.convertToUtc(parsed);
        options.value(date!);
      } else {
        options.value(null!);
      }
    };

    if (ko.isWriteableObservable(options.value)) {
      var origOnSelect = $(element).datepicker('option', 'onSelect');
      $(element).datepicker(
        'option',
        'onSelect',
        function (this: any, selectedText: string) {
          selectDate(selectedText);

          if (typeof origOnSelect === 'function') {
            origOnSelect.apply(this, Array.prototype.slice.call(arguments));
          }
        },
      );
      $(element).change(() => {
        var val = $(element).val();
        selectDate(val);
      });
    }
  },
};
