using System.Text.RegularExpressions;

namespace VocaDb.Model.Helpers
{
	public static class RelatedSitesHelper
	{
		private static readonly Regex relatedSiteUrlRegex = new Regex(@"^https?://((utaitedb\.net)|(vocadb\.net)|(touhoudb\.com))\/((Song\/Details)|S)\/\d+",
			RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

		public static bool IsRelatedSite(string url)
		{
			if (string.IsNullOrEmpty(url))
				return false;

			return relatedSiteUrlRegex.IsMatch(url);
		}
	}
}
