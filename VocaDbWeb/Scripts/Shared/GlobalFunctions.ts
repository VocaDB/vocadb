/// <reference path="GlobalValues.ts" />

module vdb.functions {

	export function getId(elem: HTMLElement) {
		if ($(elem) == null || $(elem).attr('id') == null)
			return null;

		var parts = $(elem).attr('id').split("_");
		return (parts.length >= 2 ? parts[1] : null);		
	}

    export function isNullOrWhiteSpace(str: string) {

        if (str == null || str.length == 0)
            return true;

        return !(/\S/.test(str));

    }

    export function mapAbsoluteUrl(relative: string) {

        return mergeUrls(vdb.values.baseAddress, relative);

    };

    export function mapFullUrl(relative: string) {

        return mergeUrls(vdb.values.hostAddress, relative);

    };

    export function mergeUrls(base: string, relative: string) {
        
        if (base.charAt(base.length - 1) == "/" && relative.charAt(0) == "/")
            return base + relative.substr(1);

        if (base.charAt(base.length - 1) == "/" && relative.charAt(0) != "/")
            return base + relative;

        if (base.charAt(base.length - 1) != "/" && relative.charAt(0) == "/")
            return base + relative;
        
        return base + "/" + relative;

    }

	export function trackOutboundLink(event: Event) {

		if (typeof ga !== "function" || !event.target || !navigator.sendBeacon)
			return;

		const href = (event.target as HTMLAnchorElement).href;

		if (!href)
			return;

		// Do not wait for response
		ga('send', 'event', 'outbound', 'click', href, {
			'transport': 'beacon'
		});

	}

}

interface Navigator {
	sendBeacon: Function;
}