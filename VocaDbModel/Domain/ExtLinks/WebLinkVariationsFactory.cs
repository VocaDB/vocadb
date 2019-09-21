using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Domain.ExtLinks {

	/// <summary>
	/// Gets variations of a web link URL based on <see cref="WebLinkVariationTypes"/>,
	/// for example scheme variations (http/https).
	/// </summary>
	/// <remarks>
	/// The result is intended to be used in "IN" SQL query for exact matches.
	/// SQL "LIKE" query could also be used, but it may provide unexpected results unless escaped properly.
	/// </remarks>
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

			var trimmed = originalUrl?.Trim();

			if (string.IsNullOrEmpty(trimmed))
				return new string[0];

			var variations = Enumerable.Repeat(trimmed, 1);

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
