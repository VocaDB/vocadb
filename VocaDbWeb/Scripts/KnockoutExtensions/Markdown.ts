import HtmlHelper from '@Helpers/HtmlHelper';
import $ from 'jquery';
import ko, { Observable } from 'knockout';
import _ from 'lodash';

declare global {
	interface KnockoutBindingHandlers {
		markdown: KnockoutBindingHandler;
	}
}

// Renders an observable as HTML processed by Markdown (marked library, https://github.com/chjj/marked).
// Read-only - the resulted markdown cannot be edited.
ko.bindingHandlers.markdown = {
	update: (
		element: HTMLElement,
		valueAccessor: () => Observable<string>,
		allBindingsAccessor?: () => any,
	): void => {
		const val: string = ko.unwrap(valueAccessor());
		const maxLength: number | null = allBindingsAccessor!()?.maxLength;
		const truncated = maxLength ? _.truncate(val, { length: maxLength }) : val;

		if (!val) {
			$(element).text(val);
			return;
		}

		HtmlHelper.formatMarkdown(truncated, (err, content) => {
			const text: string = err ?? content;
			$(element).html(text);
		});
	},
};
