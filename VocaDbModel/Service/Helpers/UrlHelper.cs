using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.Service.Helpers
{
	public static class UrlHelper
	{
		private static bool IsFullLink(string str)
		{
			return (str.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
				|| str.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase)
				|| str.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase));
		}

		/// <summary>
		/// Makes a proper URL from a possible URL without a http:// prefix.
		/// </summary>
		/// <param name="partialLink">Partial URL. Can be null.</param>
		/// <param name="assumeWww">Whether to assume the URL should start with www.</param>
		/// <returns>Full URL including http://. Can be null if source was null.</returns>
		[return:NotNullIfNotNull("partialLink"/* TODO: use nameof */)]
		public static string? MakeLink(string? partialLink, bool assumeWww = false)
		{
			if (string.IsNullOrEmpty(partialLink))
				return partialLink;

			if (IsFullLink(partialLink))
				return partialLink;

			if (assumeWww && !partialLink.StartsWith("www.", StringComparison.InvariantCultureIgnoreCase))
				return $"http://www.{partialLink}";

			return $"http://{partialLink}";
		}

		[return:NotNullIfNotNull("partialLink"/* TODO: use nameof */)]
		public static string? MakePossileAffiliateLink(string? partialLink)
		{
			var link = MakeLink(partialLink);

			return (new ExtSites.AffiliateLinkGenerator(new VdbConfigManager())).GenerateAffiliateLink(link);
		}

		/// <summary>
		/// Removes http:// and https:// from the beginning of an URL.
		/// </summary>
		[return:NotNullIfNotNull("url"/* TODO: use nameof */)]
		public static string? RemoveScheme(string? url)
		{
			if (string.IsNullOrEmpty(url))
				return url;

			if (url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
			{
				return url.Substring(7);
			}
			else if (url.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
			{
				return url.Substring(8);
			}

			return url;
		}

		/// <summary>
		/// List of domains/URL prefixes that can be upgraded from HTTP to HTTPS as is, for example "http://i1.sndcdn.com" -> "https://i1.sndcdn.com"
		/// </summary>
		private static readonly string[] httpUpgradeDomains = new[] {
			"http://i1.sndcdn.com", "http://nicovideo.cdn.nimg.jp/thumbnails/"
		};

		/// <summary>
		/// List of URLs that can be upgraded from HTTP to HTTPS, but require URL manipulation.
		/// </summary>
		private static readonly RegexLinkMatcher[] httpUpgradeMatchers = new[] {
			new RegexLinkMatcher("https://tn.smilevideo.jp/smile?i={0}", @"^http://tn(?:-skr\d)?\.smilevideo\.jp/smile\?i=([\d\.]+)$")
		};

		[return:NotNullIfNotNull("url"/* TODO: use nameof */)]
		public static string? UpgradeToHttps(string? url)
		{
			if (string.IsNullOrEmpty(url) || url.StartsWith("https://"))
				return url;

			if (httpUpgradeDomains.Any(m => url.StartsWith(m)))
			{
				return url.Replace("http://", "https://");
			}

			var httpUpgradeMatch = httpUpgradeMatchers
				.Select(m => new
				{
					Success = m.TryGetLinkFromUrl(url, out var formattedUrl),
					FormattedUrl = formattedUrl
				})
				.FirstOrDefault(m => m.Success);

			if (httpUpgradeMatch != null)
				url = httpUpgradeMatch.FormattedUrl;

			return url;
		}
	}
}
