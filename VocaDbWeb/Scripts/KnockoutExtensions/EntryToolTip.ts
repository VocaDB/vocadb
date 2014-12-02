/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../DataContracts/EntryRefContract.ts" />

interface KnockoutBindingHandlers {
	albumToolTip: KnockoutBindingHandler;
    artistToolTip: KnockoutBindingHandler;
    entryToolTip: KnockoutBindingHandler;
}

module vdb.knockoutExtensions {

    export function initToolTip(element, relativeUrl: string, id: number) {
        
        $(element).qtip({
            content: {
                text: 'Loading...',
                ajax: {
                    url: vdb.functions.mapAbsoluteUrl(relativeUrl),
                    type: 'GET',
                    data: { id: id }
                }
            },
            position: {
                viewport: $(window)
            },
            style: {
                classes: "tooltip-wide"
            }
        });
    
    }

}

ko.bindingHandlers.entryToolTip = {
	init: function (element, valueAccessor: () => KnockoutObservable<vdb.dataContracts.EntryRefContract>) {

		var value: vdb.dataContracts.EntryRefContract = ko.utils.unwrapObservable(valueAccessor());

		switch (value.entryType) {
			case "Album":
				vdb.knockoutExtensions.initToolTip(element, '/Album/PopupContent', value.id);
				break;
			case "Artist":
				vdb.knockoutExtensions.initToolTip(element, '/Artist/PopupContent', value.id);
				break;
		}

	}
};

ko.bindingHandlers.albumToolTip = {
	init: function (element, valueAccessor: () => KnockoutObservable<number>) {

		var id = ko.utils.unwrapObservable(valueAccessor());
		vdb.knockoutExtensions.initToolTip(element, '/Album/PopupContent', id);

	}
};

ko.bindingHandlers.artistToolTip = {
    init: function (element, valueAccessor: () => KnockoutObservable<number>) {

        var id = ko.utils.unwrapObservable(valueAccessor());
        vdb.knockoutExtensions.initToolTip(element, '/Artist/PopupContent', id);

    }
}