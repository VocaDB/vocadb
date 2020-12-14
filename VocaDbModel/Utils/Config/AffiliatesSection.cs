#nullable disable

using System.Configuration;

namespace VocaDb.Model.Utils.Config
{
	/// <summary>
	/// Configuration section for affiliate links.
	/// </summary>
	public class AffiliatesSection : ConfigurationSection
	{
		[ConfigurationProperty("amazonComAffiliateId", DefaultValue = "")]
		public string AmazonComAffiliateId
		{
			get { return (string)this["amazonComAffiliateId"]; }
			set { this["amazonComAffiliateId"] = value; }
		}

		[ConfigurationProperty("amazonJpAffiliateId", DefaultValue = "")]
		public string amazonJpAffiliateId
		{
			get { return (string)this["amazonJpAffiliateId"]; }
			set { this["amazonJpAffiliateId"] = value; }
		}

		[ConfigurationProperty("playAsiaAffiliateId", DefaultValue = "")]
		public string PlayAsiaAffiliateId
		{
			get { return (string)this["playAsiaAffiliateId"]; }
			set { this["playAsiaAffiliateId"] = value; }
		}
	}
}
