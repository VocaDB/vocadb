using System.Configuration;

namespace VocaDb.Model.Utils.Config {

	public class SiteSettingsSection : ConfigurationSection {

		public SiteSettingsSection() {
		}

		[ConfigurationProperty("blogUrl", DefaultValue = "blog.vocadb.net")]
		public string BlogUrl {
			get { return (string)this["blogUrl"]; }
			set { this["blogUrl"] = value; }
		}

		[ConfigurationProperty("ircUrl", DefaultValue = "irc.rizon.net/vocadb")]
		public string IRCUrl {
			get { return (string)this["ircUrl"]; }
			set { this["ircUrl"] = value; }			
		}

		[ConfigurationProperty("openSearchPath", DefaultValue = "/opensearch.xml")]
		public string OpenSearchPath {
			get { return (string)this["openSearchPath"]; }
			set { this["openSearchPath"] = value; }			
		}

		[ConfigurationProperty("paypalDonateCert", DefaultValue = "")]
		public string PaypalDonateCert {
			get { return (string)this["paypalDonateCert"]; }
			set { this["paypalDonateCert"] = value; }			
		}

		[ConfigurationProperty("patreonLink", DefaultValue = "")]
		public string PatreonLink {
			get { return (string)this["patreonLink"]; }
			set { this["patreonLink"] = value; }
		}

		[ConfigurationProperty("signupsDisabled", DefaultValue = "false")]
		public bool SignupsDisabled {
			get { return (bool)this["signupsDisabled"]; }
			set { this["signupsDisabled"] = value; }						
		}

		[ConfigurationProperty("sitewideAnnouncement", DefaultValue = "")]
		public string SitewideAnnouncement {
			get { return (string)this["sitewideAnnouncement"]; }
			set { this["sitewideAnnouncement"] = value; }						
		}

		[ConfigurationProperty("spotImAccId", DefaultValue = "")]
		public string SpotImAccId {
			get { return (string)this["spotImAccId"]; }
			set { this["spotImAccId"] = value; }			
		}

		[ConfigurationProperty("twitterAccountName", DefaultValue = "")]
		public string TwitterAccountName {
			get { return (string)this["twitterAccountName"]; }
			set { this["twitterAccountName"] = value; }			
		}
	}
}
