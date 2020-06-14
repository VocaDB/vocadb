using System.Configuration;

namespace VocaDb.Model.Utils.Config {

	public class SiteSettingsSection : ConfigurationSection {

		public SiteSettingsSection() {
		}

		[ConfigurationProperty("bannerUrl", DefaultValue = "")]
		public string BannerUrl {
			get => (string)this["bannerUrl"];
			set => this["bannerUrl"] = value;
		}

		[ConfigurationProperty("blogUrl", DefaultValue = "")]
		public string BlogUrl {
			get => (string)this["blogUrl"];
			set => this["blogUrl"] = value;
		}

		/// <summary>
		/// Default stylesheet, if any. Name of the CSS file, for example "TetoDB.css".
		/// </summary>
		[ConfigurationProperty("defaultStylesheet", DefaultValue = null)]
		public string DefaultStylesheet => (string)this["defaultStylesheet"];

		[ConfigurationProperty("ircUrl", DefaultValue = "irc.rizon.net/vocadb")]
		public string IRCUrl {
			get => (string)this["ircUrl"];
			set => this["ircUrl"] = value;
		}

		[ConfigurationProperty("minAlbumYear", DefaultValue = 2000)]
		public int MinAlbumYear {
			get => (int)this["minAlbumYear"];
			set => this["minAlbumYear"] = value;
		}

		[ConfigurationProperty("openSearchPath", DefaultValue = "/opensearch.xml")]
		public string OpenSearchPath {
			get => (string)this["openSearchPath"];
			set => this["openSearchPath"] = value;
		}

		[ConfigurationProperty("paypalDonateCert", DefaultValue = "")]
		public string PaypalDonateCert {
			get => (string)this["paypalDonateCert"];
			set => this["paypalDonateCert"] = value;
		}

		[ConfigurationProperty("patreonLink", DefaultValue = "")]
		public string PatreonLink {
			get => (string)this["patreonLink"];
			set => this["patreonLink"] = value;
		}

		[ConfigurationProperty("signupsDisabled", DefaultValue = "false")]
		public bool SignupsDisabled {
			get => (bool)this["signupsDisabled"];
			set => this["signupsDisabled"] = value;
		}

		[ConfigurationProperty("siteName", DefaultValue = null)]
		public string SiteName => (string)this["siteName"];

		[ConfigurationProperty("siteTitle", DefaultValue = null)]
		public string SiteTitle => (string)this["siteTitle"];

		[ConfigurationProperty("sitewideAnnouncement", DefaultValue = "")]
		public string SitewideAnnouncement {
			get => (string)this["sitewideAnnouncement"];
			set => this["sitewideAnnouncement"] = value;
		}

		/// <summary>
		/// Comma-separated list of stylesheets with extensions, for example "TetoDB.css,DarkAngel.css"
		/// </summary>
		[ConfigurationProperty("stylesheets", DefaultValue = null)]
		public string Stylesheets => (string)this["stylesheets"];

		[ConfigurationProperty("twitterAccountName", DefaultValue = "")]
		public string TwitterAccountName {
			get => (string)this["twitterAccountName"];
			set => this["twitterAccountName"] = value;
		}
	}
}
