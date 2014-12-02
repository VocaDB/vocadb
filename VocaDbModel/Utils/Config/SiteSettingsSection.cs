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

		[ConfigurationProperty("sitewideAnnouncement", DefaultValue = "")]
		public string SitewideAnnouncement {
			get { return (string)this["sitewideAnnouncement"]; }
			set { this["sitewideAnnouncement"] = value; }						
		}


	}
}
