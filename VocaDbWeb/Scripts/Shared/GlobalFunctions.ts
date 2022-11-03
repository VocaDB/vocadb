import $ from 'jquery';

export class functions {
	static getId(elem: HTMLElement): string | null {
		if ($(elem) == null || $(elem).attr('id') == null) return null;

		var parts = $(elem).attr('id').split('_');
		return parts.length >= 2 ? parts[1] : null;
	}

	static isNullOrWhiteSpace(str: string): boolean {
		if (str == null || str.length === 0) return true;

		return !/\S/.test(str);
	}

	static mapAbsoluteUrl(relative: string): string {
		return functions.mergeUrls(vdb.values.baseAddress, relative);
	}

	static mergeUrls(base: string, relative: string): string {
		if (base.charAt(base.length - 1) === '/' && relative.charAt(0) === '/')
			return base + relative.substr(1);

		if (base.charAt(base.length - 1) === '/' && relative.charAt(0) !== '/')
			return base + relative;

		if (base.charAt(base.length - 1) !== '/' && relative.charAt(0) === '/')
			return base + relative;

		return base + '/' + relative;
	}

	static getUrlDomain(url: string): string {
		// http://stackoverflow.com/a/8498629
		const matches = url ? url.match(/^https?:\/\/([^/?#]+)(?:[/?#]|$)/i) : null;
		return matches! && matches[1]; // domain will be null if no match is found
	}

	static trackOutboundLink(event: MouseEvent): void {
		// Do nothing.
	}

	static disableTabReload = function (tab: any): void {
		tab.find('a').attr('href', '#' + tab.attr('aria-controls'));
	};

	static showLoginPopup = function (): void {
		$.get(
			'/User/LoginForm',
			{ returnUrl: window.location.href },
			function (result) {
				$('#loginPopup').html(result);
				$('#loginPopup').dialog('open');
			},
		);
	};
}

declare global {
	interface Navigator {
		// sendBeacon is not available in older TS versions
		sendBeacon(url: any, data?: any): boolean;
	}
}
