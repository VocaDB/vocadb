/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../typings/knockout/knockout.d.ts" />
/// <reference path="../Shared/GlobalFunctions.ts" />
/// <reference path="../DataContracts/EntryRefContract.ts" />

interface KnockoutBindingHandlers {
	albumToolTip: KnockoutBindingHandler;
    artistToolTip: KnockoutBindingHandler;
    entryToolTip: KnockoutBindingHandler;
	eventToolTip: KnockoutBindingHandler;
	songToolTip: KnockoutBindingHandler;
	tagToolTip: KnockoutBindingHandler;
	userToolTip: KnockoutBindingHandler;
}

module vdb.knockoutExtensions {

	export function initToolTip(element: HTMLElement, relativeUrl: string, id: number, params?: any, foreignDomain?: string) {

		const whitelistedDomains = ["http://vocadb.net", "https://vocadb.net", "http://utaitedb.net"];
		const url = foreignDomain && _.some(whitelistedDomains, domain => _.includes(foreignDomain.toLocaleLowerCase(), domain)) ?
			vdb.functions.mergeUrls(foreignDomain, relativeUrl) : vdb.functions.mapAbsoluteUrl(relativeUrl);
		const data = _.assign({ id: id }, params);

        $(element).qtip({
            content: {
                text: 'Loading...',
                ajax: {
					url: url,
                    type: 'GET',
					data: data,
					dataType: foreignDomain ? 'jsonp' : undefined
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

	export interface TooltipOptions {
		toolTipDomain?: string;
		version?: number;
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

ko.bindingHandlers.eventToolTip = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<number>) => {
		const culture = vdb.values.uiLanguage || undefined;
		vdb.knockoutExtensions.initToolTip(element, '/Event/PopupContent', ko.unwrap(valueAccessor()), { culture: culture });
	}
}

ko.bindingHandlers.songToolTip = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<number>, allPropertiesAccessor: () => vdb.knockoutExtensions.TooltipOptions) => {
		const allProps = allPropertiesAccessor();
		vdb.knockoutExtensions.initToolTip(element, '/Song/PopupContentWithVote', ko.unwrap(valueAccessor()), { version: allProps.version }, allProps.toolTipDomain);
	}
}

ko.bindingHandlers.tagToolTip = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<number>) => {
		var culture = vdb.values.uiLanguage || undefined;
		var lang = vdb.models.globalization.ContentLanguagePreference[vdb.values.languagePreference] || undefined;
		vdb.knockoutExtensions.initToolTip(element, '/Tag/PopupContent', ko.unwrap(valueAccessor()), { culture: culture, lang: lang });
	}
}

ko.bindingHandlers.userToolTip = {
	init: (element: HTMLElement, valueAccessor: () => KnockoutObservable<number>) => {
		var culture = vdb.values.uiLanguage || undefined;
		vdb.knockoutExtensions.initToolTip(element, '/User/PopupContent', ko.unwrap(valueAccessor()), { culture: culture });
	}
}