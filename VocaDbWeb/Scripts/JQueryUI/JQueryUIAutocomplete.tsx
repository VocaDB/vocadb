import $ from 'jquery';
import 'jquery-ui';
import React, { useImperativeHandle } from 'react';

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

	const options = React.useMemo(() => ({ select, source }), [select, source]);

	React.useLayoutEffect(() => {
		const $el = $(el.current);
		const autocomplete = $el.autocomplete(options).data('ui-autocomplete');
		if (renderItem) autocomplete._renderItem = renderItem;
		return (): void => $el.autocomplete('destroy');
	}, [renderItem, options]);

	return <input ref={el} {...props} />;
});

export default JQueryUIAutocomplete;
