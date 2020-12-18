#nullable disable

using System.Configuration;

namespace VocaDb.SiteMapGenerator
{
	public class Config
	{
		private string AppSetting(string key, string def)
		{
			return ConfigurationManager.AppSettings[key] ?? def;
		}

		public string OutFolder => AppSetting("outFolder", string.Empty);

		public string SiteRootUrl => AppSetting("siteRootUrl", "http://vocadb.net/");

		public string SitemapRootUrl => AppSetting("sitemapRootUrl", "http://static.vocadb.net/sitemaps/");
	}
}
