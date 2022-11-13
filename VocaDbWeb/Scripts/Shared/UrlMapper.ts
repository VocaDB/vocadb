import { functions } from '@/Shared/GlobalFunctions';

export class UrlMapper {
	static buildUrl = (...args: string[]): string => {
		return args.reduce((list: string, item: string) =>
			UrlMapper.mergeUrls(list, item),
		)!;
	};

	static mergeUrls = (base: string, relative: string): string => {
		if (base.charAt(base.length - 1) === '/' && relative.charAt(0) === '/')
			return base + relative.substr(1);

		if (base.charAt(base.length - 1) === '/' && relative.charAt(0) !== '/')
			return base + relative;

		if (base.charAt(base.length - 1) !== '/' && relative.charAt(0) === '/')
			return base + relative;

		return base + '/' + relative;
	};

	constructor(readonly baseUrl: string) {}

	mapRelative(relative: string): string {
		return functions.mergeUrls(this.baseUrl, relative);
	}
}

export const urlMapper = new UrlMapper(vdb.values.baseAddress);
