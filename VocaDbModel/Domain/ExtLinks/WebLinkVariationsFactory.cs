using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Domain.ExtLinks
{

	public static class WebLinkVariationsFactory {

		private static IEnumerable<string> GetSchemeAgnostic(string url) {
			var urlTrimmed = UrlHelper.RemoveScheme(url);
			return new[] {
				urlTrimmed,
				$"http://{urlTrimmed}",
				$"https://{urlTrimmed}"
			};
		}

		private static IEnumerable<string> GetSlashAgnostic(string url) {
			var withoutSlash = url.EndsWith("/") ? url.Substring(0, url.Length - 1) : url;
			return new[] { 
				withoutSlash,
				withoutSlash + "/"
			};			
		}

		public static string[] GetWebLinkVariations(string originalUrl, WebLinkVariationTypes variationTypes) {

			if (string.IsNullOrEmpty(originalUrl))
				return new string[0];

			var trimmed = originalUrl.Trim();

			if (variationTypes == WebLinkVariationTypes.Nothing)
			{
				return new[] { trimmed };
			}

			var variations = Enumerable.Empty<string>();

			if (variationTypes.HasFlag(WebLinkVariationTypes.IgnoreScheme))
			{
				variations = GetSchemeAgnostic(trimmed);
			}

			if (variationTypes.HasFlag(WebLinkVariationTypes.IgnoreTrailingSlash))
			{
				variations = variations.SelectMany(v => GetSlashAgnostic(v));
			}

			return variations.ToArray();

		}

	}
}
