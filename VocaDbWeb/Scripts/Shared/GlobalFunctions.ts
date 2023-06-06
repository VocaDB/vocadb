import { vdbConfig } from '@/vdbConfig';

export class functions {
	static isNullOrWhiteSpace(str: string): boolean {
		if (str == null || str.length === 0) return true;

		return !/\S/.test(str);
	}

	static mapAbsoluteUrl(relative: string): string {
		return functions.mergeUrls(vdbConfig.baseAddress, relative);
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

	static trackOutboundLink(event: MouseEvent): void {
		// Do nothing.
	}
}

declare global {
	interface Navigator {
		// sendBeacon is not available in older TS versions
		sendBeacon(url: any, data?: any): boolean;
	}
}
