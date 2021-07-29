import $ from 'jquery';
import 'jquery-ui';
import React, { useImperativeHandle } from 'react';

const useJQueryUIAutocomplete = (
	el: React.RefObject<any>,
	options: JQueryUI.AutocompleteOptions,
	renderItem?: (ul: HTMLElement, item: any) => JQuery,
): void => {
	React.useEffect(() => {
		const $el = $(el.current);
		const autocomplete = $el.autocomplete(options).data('ui-autocomplete');
		if (renderItem) autocomplete._renderItem = renderItem;
		return (): void => $el.autocomplete('destroy');
	});
};

type JQueryUIAutocompleteProps = {
	renderItem?: (ul: HTMLElement, item: any) => JQuery;
} & JQueryUI.AutocompleteOptions &
	React.InputHTMLAttributes<HTMLInputElement>;

const JQueryUIAutocomplete = React.forwardRef<
	HTMLInputElement,
	React.PropsWithChildren<JQueryUIAutocompleteProps>
>(({ select, source, renderItem, ...props }, ref) => {
	const el = React.useRef<HTMLInputElement>(undefined!);
	useImperativeHandle<HTMLInputElement, HTMLInputElement>(
		ref,
		() => el.current,
	);
	useJQueryUIAutocomplete(el, { select, source }, renderItem);

	return <input ref={el} {...props} />;
});

export default JQueryUIAutocomplete;
