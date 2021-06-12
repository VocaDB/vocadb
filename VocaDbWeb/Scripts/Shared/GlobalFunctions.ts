import $ from 'jquery';

import VocaDbContext from './VocaDbContext';
import { container } from './inversify.config';

const vocaDbContext = container.get(VocaDbContext);

export default class functions {
	public static getId(elem: HTMLElement): string | null {
		if ($(elem) == null || $(elem).attr('id') == null) return null;

		var parts = $(elem).attr('id').split('_');
		return parts.length >= 2 ? parts[1] : null;
	}

	public static isNullOrWhiteSpace(str: string): boolean {
		if (str == null || str.length === 0) return true;

		return !/\S/.test(str);
	}

	public static mapAbsoluteUrl(relative: string): string {
		return functions.mergeUrls(vocaDbContext.baseAddress, relative);
	}

	public static mergeUrls(base: string, relative: string): string {
		if (base.charAt(base.length - 1) === '/' && relative.charAt(0) === '/')
			return base + relative.substr(1);

		if (base.charAt(base.length - 1) === '/' && relative.charAt(0) !== '/')
			return base + relative;

		if (base.charAt(base.length - 1) !== '/' && relative.charAt(0) === '/')
			return base + relative;

		return base + '/' + relative;
	}

	public static getUrlDomain(url: string): string {
		// http://stackoverflow.com/a/8498629
		const matches = url ? url.match(/^https?:\/\/([^/?#]+)(?:[/?#]|$)/i) : null;
		return matches! && matches[1]; // domain will be null if no match is found
	}

	public static trackOutboundLink(event: MouseEvent): void {
		// Skip tracking if ga not present, sendBeacon is not supported, or mouse button is right-click
		const mright = 2;
		if (
			typeof ga !== 'function' ||
			!event ||
			!event.target ||
			!navigator.sendBeacon ||
			event.button === mright
		)
			return;

		const href = (event.target as HTMLAnchorElement).href;

		if (!href) return;

		const domain = functions.getUrlDomain(href);

		// Beacon transport doesn't require waiting for response
		// https://developers.google.com/analytics/devguides/collection/analyticsjs/sending-hits#specifying_different_transport_mechanisms
		ga('send', 'event', 'outbound', 'click', href, {
			transport: 'beacon',
			dimension1: domain,
		});
	}
}

declare global {
	interface Navigator {
		// sendBeacon is not available in older TS versions
		sendBeacon(url: any, data?: any): boolean;
	}
}
