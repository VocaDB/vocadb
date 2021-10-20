import GlobalValues from '@Shared/GlobalValues';

// Corresponds to the AffiliateLinkGenerator class in C#.
/// <summary>
/// Generates affiliate (paid) links to partner sites.
/// </summary>
export class AffiliateLinkGenerator {
	private readonly amazonComAffId: string;
	private readonly amazonJpAffId: string;
	private readonly paAffId: string;

	private addOrReplaceParam = (
		url: string,
		affIdRegex: string | undefined,
		param: string | undefined,
		val: string | undefined,
	): string => {
		const paramEq = `${param}=`;

		if (url.includes(paramEq)) {
			return url.replace(
				new RegExp(`${paramEq}${affIdRegex}`),
				`${paramEq}${val}`,
			);
		} else if (url.includes('?')) {
			return `${url}&${paramEq}${val}`;
		} else {
			return `${url}?${paramEq}${val}`;
		}
	};

	private replaceAmazonComLink = (url: string): string => {
		if (!this.amazonComAffId || !url.includes('www.amazon.com/')) return url;

		return this.addOrReplaceParam(url, '(\\w+)', 'tag', this.amazonComAffId);
	};

	private replaceAmazonJpLink = (url: string): string => {
		if (!this.amazonJpAffId || !url.includes('www.amazon.co.jp/')) return url;

		return this.addOrReplaceParam(url, '(\\w+)', 'tag', this.amazonJpAffId);
	};

	private replacePlayAsiaLink = (url: string): string => {
		if (!this.paAffId || !url.includes('www.play-asia.com/')) return url;

		return this.addOrReplaceParam(url, '(\\d+)', 'affiliate_id', this.paAffId);
	};

	public constructor(values: GlobalValues) {
		this.amazonComAffId = values.amazonComAffiliateId;
		this.amazonJpAffId = values.amazonJpAffiliateId;
		this.paAffId = values.playAsiaAffiliateId;
	}

	public generateAffiliateLink = (url?: string): string | undefined => {
		if (!url) return url;

		url = this.replaceAmazonComLink(url);
		url = this.replaceAmazonJpLink(url);
		url = this.replacePlayAsiaLink(url);

		return url;
	};
}

// Corresponds to the UrlHelper class in C#.
export default class UrlHelper {
	private static isFullLink = (str: string): boolean => {
		return (
			str.startsWith('http://') ||
			str.startsWith('https://') ||
			str.startsWith('mailto:')
		);
	};

	/// <summary>
	/// Makes a proper URL from a possible URL without a http:// prefix.
	/// </summary>
	/// <param name="partialLink">Partial URL. Can be null.</param>
	/// <param name="assumeWww">Whether to assume the URL should start with www.</param>
	/// <returns>Full URL including http://. Can be null if source was null.</returns>
	public static makeLink = (
		partialLink: string | undefined,
		assumeWww: boolean = false,
	): string | undefined => {
		if (!partialLink) return partialLink;

		if (UrlHelper.isFullLink(partialLink)) return partialLink;

		if (assumeWww && !partialLink.startsWith('www.'))
			return `http://www.${partialLink}`;

		return `http://${partialLink}`;
	};

	public static makePossileAffiliateLink = (
		partialLink?: string,
	): string | undefined => {
		const link = UrlHelper.makeLink(partialLink);

		return new AffiliateLinkGenerator(vdb.values).generateAffiliateLink(link);
	};
}
