import DateTimeHelper from '@Helpers/DateTimeHelper';
import $ from 'jquery';
import 'jquery-ui';
import React, { useImperativeHandle } from 'react';

const useJQueryUIDatepicker = (
	el: React.RefObject<any>,
	options: JQueryUI.DatepickerOptions,
	value?: Date,
	onSelect?: (date?: Date) => void,
): void => {
	React.useEffect(() => {
		const $el = $(el.current);
		$el.datepicker(options);

		if (value) {
			$el.datepicker('setDate', DateTimeHelper.convertToLocal(value));
		}

		const selectDate = (selectedText: string): void => {
			if (selectedText) {
				const format = $el.datepicker('option', 'dateFormat');
				const parsed = $.datepicker.parseDate(format, selectedText);
				// Make sure the date is parsed as UTC as we don't want any timezones here. jQuery UI seems to always parse as local.
				const date = DateTimeHelper.convertToUtc(parsed);
				onSelect?.(date!);
			} else {
				onSelect?.(undefined);
			}
		};

		const origOnSelect = $el.datepicker('option', 'onSelect');
		$el.datepicker(
			'option',
			'onSelect',
			function (this: any, selectedText: string) {
				selectDate(selectedText);

				if (typeof origOnSelect === 'function') {
					origOnSelect.apply(this, Array.prototype.slice.call(arguments));
				}
			},
		);
		$el.change(() => {
			const val = $el.val();
			selectDate(val);
		});

		return (): void => {
			$el.datepicker('destroy');
			$el.off('change');
		};
	});
};

type JQueryUIDatepickerProps = {
	value?: Date;
	onSelect?: (date?: Date) => void;
} & Omit<JQueryUI.DatepickerOptions, 'onSelect'> &
	Omit<React.InputHTMLAttributes<HTMLInputElement>, 'value' | 'onSelect'>;

const JQueryUIDatepicker = React.forwardRef<
	HTMLInputElement,
	React.PropsWithChildren<JQueryUIDatepickerProps>
>(({ dateFormat, value, onSelect, ...props }, ref) => {
	const el = React.useRef<HTMLInputElement>(undefined!);
	useImperativeHandle<HTMLInputElement, HTMLInputElement>(
		ref,
		() => el.current,
	);
	useJQueryUIDatepicker(el, { dateFormat: dateFormat }, value, onSelect);

	return <input ref={el} {...props} />;
});

export default JQueryUIDatepicker;
