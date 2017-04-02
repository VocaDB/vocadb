using System.Web.Configuration;

namespace VocaDb.Model.Utils.Config {

	/// <summary>
	/// Manages VocaDb global configuration.
	/// </summary>
	public class VdbConfigManager {

		public AffiliatesSection Affiliates {
			get {
				var section = (AffiliatesSection)WebConfigurationManager.GetSection("vocaDb/affiliates");
				return section ?? new AffiliatesSection();
			}
		}

		public SiteSettingsSection SiteSettings => (SiteSettingsSection)WebConfigurationManager.GetSection("vocaDb/siteSettings") ?? new SiteSettingsSection();

		public SpecialTagsSection SpecialTags
		{
			get
			{
				var section = (SpecialTagsSection)WebConfigurationManager.GetSection("vocaDb/specialTags");
				return section ?? new SpecialTagsSection();
			}
		}
	}

}
