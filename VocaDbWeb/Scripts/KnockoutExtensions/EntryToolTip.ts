/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../DataContracts/EntryRefContract.ts" />

interface KnockoutBindingHandlers {
	albumToolTip: KnockoutBindingHandler;
    artistToolTip: KnockoutBindingHandler;
    entryToolTip: KnockoutBindingHandler;
	songToolTip: KnockoutBindingHandler;
	tagToolTip: KnockoutBindingHandler;
}

module vdb.knockoutExtensions {

	export function initToolTip(element: HTMLElement, relativeUrl: string, id: number) {
        
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
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<vdb.dataContracts.EntryRefContract>) => {

		var value: vdb.dataContracts.EntryRefContract = ko.unwrap(valueAccessor());

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
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<number>) => {
		vdb.knockoutExtensions.initToolTip(element, '/Album/PopupContent', ko.unwrap(valueAccessor()));
	}
};

ko.bindingHandlers.artistToolTip = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<number>) => {
		vdb.knockoutExtensions.initToolTip(element, '/Artist/PopupContent', ko.unwrap(valueAccessor()));
    }
}

ko.bindingHandlers.songToolTip = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<number>) => {
		vdb.knockoutExtensions.initToolTip(element, '/Song/PopupContent', ko.unwrap(valueAccessor()));
	}
}

ko.bindingHandlers.tagToolTip = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<number>) => {
		vdb.knockoutExtensions.initToolTip(element, '/Tag/PopupContent', ko.unwrap(valueAccessor()));
	}
}