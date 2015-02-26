/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../typings/jqueryui/jqueryui.d.ts" />

interface KnockoutBindingHandlers {
    dialog: KnockoutBindingHandler;
}

// Code from http://stackoverflow.com/questions/8611327/integrating-jquery-ui-dialog-with-knockoutjs/8611892#8611892
ko.bindingHandlers.dialog = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var options: any = ko.utils.unwrapObservable(valueAccessor()) || {};
        //do in a setTimeout, so the applyBindings doesn't bind twice from element being copied and moved to bottom
        setTimeout(function () {
            options.close = function () {
                allBindingsAccessor().dialogVisible(false);
            };

            var diag = $(element).dialog(options);
			$(element).data("dialog", diag);
        }, 0);

        //handle disposal (not strictly necessary in this scenario)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).dialog("destroy");
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        var shouldBeOpen = ko.utils.unwrapObservable(allBindingsAccessor().dialogVisible),
            $el = $(element),
            dialog = $el.data("uiDialog") || $el.data("dialog");

        var dialogButtons = ko.utils.unwrapObservable(allBindingsAccessor().dialogButtons);

        //don't call open/close before initilization
        if (dialog) {
            $el.dialog("option", "buttons", dialogButtons);
            $el.dialog(shouldBeOpen ? "open" : "close");
        }
    }
};