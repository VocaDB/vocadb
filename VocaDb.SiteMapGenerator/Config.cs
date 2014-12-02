using System.Configuration;

namespace VocaDb.SiteMapGenerator {
	public class Config {

		private string AppSetting(string key, string def) {
			return ConfigurationManager.AppSettings[key] ?? def;
		}

		public string OutFolder {
			get { return AppSetting("outFolder", string.Empty); }
		}

		public string SiteRootUrl {
			get { return AppSetting("siteRootUrl", "http://vocadb.net/"); }
		}

		public string SitemapRootUrl {
			get { return AppSetting("sitemapRootUrl", "http://static.vocadb.net/sitemaps/"); }
		}

	}
}
