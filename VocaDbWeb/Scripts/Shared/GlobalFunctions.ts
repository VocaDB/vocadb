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

	export function getUrlDomain(url: string) {
		// http://stackoverflow.com/a/8498629
		const matches = url ? url.match(/^https?\:\/\/([^\/?#]+)(?:[\/?#]|$)/i) : null;
		return matches && matches[1];  // domain will be null if no match is found		
	}

	export function trackOutboundLink(event: MouseEvent) {

		// Skip tracking if ga not present, sendBeacon is not supported, or mouse button is right-click
		const mright = 2;
		if (typeof ga !== "function" || !event || !event.target || !navigator.sendBeacon || event.button === mright)
			return;

		const href = (event.target as HTMLAnchorElement).href;

		if (!href)
			return;

		const domain = getUrlDomain(href);

		// Beacon transport doesn't require waiting for response
		// https://developers.google.com/analytics/devguides/collection/analyticsjs/sending-hits#specifying_different_transport_mechanisms
		ga('send', 'event', 'outbound', 'click', href, {
			'transport': 'beacon',
			'dimension1': domain
		});

	}

}

interface Navigator {
	sendBeacon: Function;
}