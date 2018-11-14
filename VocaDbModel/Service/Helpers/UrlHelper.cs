using System;
using System.Text.RegularExpressions;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.Service.Helpers {

	public static class UrlHelper {

		private static bool IsFullLink(string str) {

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
		public static string MakeLink(string partialLink, bool assumeWww = false) {

			if (string.IsNullOrEmpty(partialLink))
				return partialLink;

			if (IsFullLink(partialLink))
				return partialLink;

			if (assumeWww && !partialLink.StartsWith("www.", StringComparison.InvariantCultureIgnoreCase))
				return string.Format("http://www.{0}", partialLink);

			return string.Format("http://{0}", partialLink);

		}

		public static string MakePossileAffiliateLink(string partialLink) {

			var link = MakeLink(partialLink);

			return (new ExtSites.AffiliateLinkGenerator(new VdbConfigManager())).GenerateAffiliateLink(link);

		}
		
		/// <summary>
		/// Removes http:// and https:// from the beginning of an URL.
		/// </summary>
		public static string RemoveScheme(string url) {

			if (string.IsNullOrEmpty(url))
				return url;

			if (url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)) {
				return url.Substring(7);
			} else if (url.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase)) {
				return url.Substring(8);
			}

			return url;

		}

		private static readonly Regex nicoImageRegex = new Regex(@"^http://tn(?:-skr\d)?\.smilevideo\.jp/smile\?i=([\d\.]+)$");

		public static string UpgradeToHttps(string url) {

			if (string.IsNullOrEmpty(url))
				return url;

			if (url.StartsWith("http://i1.sndcdn.com")) {
				return url.Replace("http://", "https://");
			}

			var nicoRegexMatch = nicoImageRegex.Match(url);

			if (nicoRegexMatch.Success) {
				return string.Format("https://tn.smilevideo.jp/smile?i=" + nicoRegexMatch.Groups[1]);
			}

			return url;

		}

	}

}
