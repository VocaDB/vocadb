import { EntryThumbContract } from '@/types/DataContracts/EntryThumbContract';
import { RegexLinkMatcher } from '@/Helpers/RegexLinkMatcher';
import { ImageSize } from '@/types/Models/Images/ImageSize';

// Corresponds to the UrlHelper and UrlHelperExtensionsForImages classes in C#.
export class UrlHelper {
	private static isFullLink = (str: string): boolean => {
		return str.startsWith('http://') || str.startsWith('https://') || str.startsWith('mailto:');
	};

	/// <summary>
	/// Makes a proper URL from a possible URL without a http:// prefix.
	/// </summary>
	/// <param name="partialLink">Partial URL. Can be null.</param>
	/// <param name="assumeWww">Whether to assume the URL should start with www.</param>
	/// <returns>Full URL including http://. Can be null if source was null.</returns>
	static makeLink = (
		partialLink: string | undefined,
		assumeWww: boolean = false
	): string | undefined => {
		if (!partialLink) return partialLink;

		if (UrlHelper.isFullLink(partialLink)) return partialLink;

		if (assumeWww && !partialLink.startsWith('www.')) return `http://www.${partialLink}`;

		return `http://${partialLink}`;
	};

	/// <summary>
	/// List of domains/URL prefixes that can be upgraded from HTTP to HTTPS as is, for example "http://i1.sndcdn.com" -> "https://i1.sndcdn.com"
	/// </summary>
	private static readonly httpUpgradeDomains = [
		'http://i1.sndcdn.com',
		'http://nicovideo.cdn.nimg.jp/thumbnails/',
	];

	/// <summary>
	/// List of URLs that can be upgraded from HTTP to HTTPS, but require URL manipulation.
	/// </summary>
	private static readonly httpUpgradeMatchers = [
		new RegexLinkMatcher(
			'https://tn.smilevideo.jp/smile?i={0}',
			'^http://tn(?:-skr\\d)?\\.smilevideo\\.jp/smile\\?i=([\\d\\.]+)$'
		),
	];

	static upgradeToHttps = (url?: string): string | undefined => {
		if (!url || url.startsWith('https://')) return url;

		if (UrlHelper.httpUpgradeDomains.some((m) => url!.startsWith(m)))
			return url.replace('http://', 'https://');

		const [httpUpgradeMatch] = UrlHelper.httpUpgradeMatchers
			.map((m) => m.getLinkFromUrl(url!))
			.filter((m) => m.success);

		if (httpUpgradeMatch) url = httpUpgradeMatch.formattedUrl;

		return url;
	};

	static getSmallestThumb = (
		imageInfo: EntryThumbContract,
		preferLargerThan: ImageSize
	): string | undefined => {
		switch (preferLargerThan) {
			case ImageSize.TinyThumb:
				return (
					imageInfo.urlTinyThumb ??
					imageInfo.urlSmallThumb ??
					imageInfo.urlThumb ??
					imageInfo.urlOriginal
				);

			case ImageSize.SmallThumb:
				return (
					imageInfo.urlSmallThumb ??
					imageInfo.urlThumb ??
					imageInfo.urlTinyThumb ??
					imageInfo.urlOriginal
				);

			case ImageSize.Thumb:
				return (
					imageInfo.urlThumb ??
					imageInfo.urlSmallThumb ??
					imageInfo.urlTinyThumb ??
					imageInfo.urlOriginal
				);

			default:
				return (
					imageInfo.urlOriginal ??
					imageInfo.urlThumb ??
					imageInfo.urlSmallThumb ??
					imageInfo.urlTinyThumb
				);
		}
	};

	static imageThumb = (
		imageInfo: EntryThumbContract | undefined,
		size: ImageSize,
		useUnknownImage = true
	): string => {
		return (
			(imageInfo && UrlHelper.getSmallestThumb(imageInfo, size)) ||
			(useUnknownImage ? '/Content/unknown.png' : '')
		);
	};
}
