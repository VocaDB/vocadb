/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />

interface KnockoutBindingHandlers {
    // Binding handler for jQuery focusout event.
    focusout: KnockoutBindingHandler;
}

ko.bindingHandlers.focusout = {
    init: function (element, valueAccessor) {
        var value: any = ko.utils.unwrapObservable(valueAccessor());
        $(element).focusout(value);
    }
};
