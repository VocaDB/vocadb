/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />

interface KnockoutBindingHandlers {

    jqButton: KnockoutBindingHandler;

}

interface jqButtonParams {

    disabled?: boolean;

    icon?: string;

}

ko.bindingHandlers.jqButton = {
	init: (element: HTMLElement, valueAccessor: () => jqButtonParams, allBindingsAccessor: () => any) => {

		var params: jqButtonParams = ko.unwrap(valueAccessor()) || {};
		var allBindings = allBindingsAccessor();
		var disable = ko.unwrap(allBindings.disable) || params.disabled;

		$(element).button({ disabled: disable, icons: { primary: params.icon } });

	},
	update: (element: HTMLElement, valueAccessor: () => jqButtonParams, allBindingsAccessor: () => any) => {

		var params: jqButtonParams = ko.unwrap(valueAccessor()) || {};
		var allBindings = allBindingsAccessor();
		var disable = ko.unwrap(allBindings.disable) || params.disabled;

		$(element).button("option", "disabled", disable);
			
	}
}