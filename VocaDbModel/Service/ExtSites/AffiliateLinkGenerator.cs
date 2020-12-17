#nullable disable

using System.Text.RegularExpressions;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.Service.ExtSites
{
	/// <summary>
	/// Generates affiliate (paid) links to partner sites.
	/// </summary>
	public class AffiliateLinkGenerator
	{
		/*private static readonly RegexLinkMatcher cdjRegex = new RegexLinkMatcher("http://www.cdjapan.co.jp/aff/click.cgi/PytJTGW7Lok/4412/A585851/detailview.html?{0}",
			@"http://www.cdjapan\.co\.jp/detailview.html\?(KEY=\w+-\d+)");*/

		private readonly string amazonComAffId;
		private readonly string amazonJpAffId;
		private readonly string paAffId;

		private string AddOrReplaceParam(string url, string affIdRegex, string param, string val)
		{
			var paramEq = param + "=";

			if (url.Contains(paramEq))
			{
				return Regex.Replace(url, paramEq + affIdRegex, string.Format(@"{0}{1}", paramEq, val));
			}
			else if (url.Contains("?"))
			{
				return $"{url}&{paramEq}{val}";
			}
			else
			{
				return $"{url}?{paramEq}{val}";
			}
		}

		private string ReplaceAmazonComLink(string url)
		{
			if (string.IsNullOrEmpty(amazonComAffId) || !url.Contains("www.amazon.com/"))
				return url;

			return AddOrReplaceParam(url, @"(\w+)", "tag", amazonComAffId);
		}

		private string ReplaceAmazonJpLink(string url)
		{
			if (string.IsNullOrEmpty(amazonJpAffId) || !url.Contains("www.amazon.co.jp/"))
				return url;

			return AddOrReplaceParam(url, @"(\w+)", "tag", amazonJpAffId);
		}

		private string ReplacePlayAsiaLink(string url)
		{
			if (string.IsNullOrEmpty(paAffId) || !url.Contains("www.play-asia.com/"))
				return url;

			return AddOrReplaceParam(url, @"(\d+)", "affiliate_id", paAffId);
		}

		public AffiliateLinkGenerator(VdbConfigManager configManager)
		{
			amazonComAffId = configManager.Affiliates.AmazonComAffiliateId;
			amazonJpAffId = configManager.Affiliates.amazonJpAffiliateId;
			paAffId = configManager.Affiliates.PlayAsiaAffiliateId;
		}

		public string GenerateAffiliateLink(string url)
		{
			if (string.IsNullOrEmpty(url))
				return url;

			/*if (cdjRegex.IsMatch(url)) {
				return cdjRegex.MakeLinkFromUrl(url);				
			}*/

			url = ReplacePlayAsiaLink(url);
			url = ReplaceAmazonComLink(url);
			url = ReplaceAmazonJpLink(url);

			return url;
		}
	}
}
