using System.Configuration;

namespace VocaDb.Model.Utils.Config {

	/// <summary>
	/// Manages VocaDb global configuration.
	/// </summary>
	public class VdbConfigManager {

		public AffiliatesSection Affiliates {
			get {
				var section = (AffiliatesSection)ConfigurationManager.GetSection("vocaDb/affiliates");
				return section ?? new AffiliatesSection();
			}
		}

		public AssetsSection Assets => (AssetsSection)ConfigurationManager.GetSection("vocaDb/assets") ?? new AssetsSection();

		public SiteSettingsSection SiteSettings => (SiteSettingsSection)ConfigurationManager.GetSection("vocaDb/siteSettings") ?? new SiteSettingsSection();

		public SpecialTagsSection SpecialTags
		{
			get
			{
				var section = (SpecialTagsSection)ConfigurationManager.GetSection("vocaDb/specialTags");
				return section ?? new SpecialTagsSection();
			}
		}
	}

}
