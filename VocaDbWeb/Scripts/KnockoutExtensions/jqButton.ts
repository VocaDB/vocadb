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
    init: (element, valueAccessor) => {

        var params: jqButtonParams = ko.utils.unwrapObservable(valueAccessor());

        $(element).button({ disabled: params.disabled, icons: { primary: params.icon } });

    }
}