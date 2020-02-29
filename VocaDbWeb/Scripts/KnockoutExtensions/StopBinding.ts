/// <reference path="../typings/knockout/knockout.d.ts" />

interface KnockoutBindingHandlers {

    stopBinding: KnockoutBindingHandler;

}

// Stops automatic Knockout binding for child elements.
// See http://www.knockmeout.net/2012/05/quick-tip-skip-binding.html
ko.bindingHandlers.stopBinding = {
    init: () => {
        return { controlsDescendantBindings: true };
    }
};

// https://knockoutjs.com/documentation/custom-bindings-for-virtual-elements.html
ko.virtualElements.allowedBindings.stopBinding = true;