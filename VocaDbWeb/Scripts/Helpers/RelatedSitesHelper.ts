export default class RelatedSitesHelper {
	private static relatedSiteUrlRegex = new RegExp(
		/^https?:\/\/((utaitedb\.net)|(vocadb\.net)|(touhoudb\.com))\/((Song\/Details)|S)\/\d+/,
		'i',
	);

	public static isRelatedSite = (url?: string): boolean => {
		if (!url) return false;

		return this.relatedSiteUrlRegex.test(url);
	};
}
