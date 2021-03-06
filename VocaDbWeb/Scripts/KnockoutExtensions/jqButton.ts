import $ from 'jquery';
import ko from 'knockout';

declare global {
	interface KnockoutBindingHandlers {
		jqButton: KnockoutBindingHandler;
	}
}

interface jqButtonParams {
	disabled?: boolean;

	icon?: string;
}

ko.bindingHandlers.jqButton = {
	init: (
		element: HTMLElement,
		valueAccessor: () => jqButtonParams,
		allBindingsAccessor?: () => any,
	): void => {
		var params: jqButtonParams = ko.unwrap(valueAccessor()) || {};
		var allBindings = allBindingsAccessor!();
		var disable = ko.unwrap(allBindings.disable) || params.disabled || false;

		$(element).button({ disabled: disable, icons: { primary: params.icon } });
	},
	update: (
		element: HTMLElement,
		valueAccessor: () => jqButtonParams,
		allBindingsAccessor?: () => any,
	): void => {
		var params: jqButtonParams = ko.unwrap(valueAccessor()) || {};
		var allBindings = allBindingsAccessor!();
		var disable = ko.unwrap(allBindings.disable) || params.disabled || false;

		$(element).button('option', 'disabled', disable);
	},
};
