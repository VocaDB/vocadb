using System.Resources;

namespace VocaDb.Model.Service.BrandableStrings.Collections {

	public class LayoutStrings {

		public LayoutStrings(ResourceManager resourceMan) {
			ResourceManager = resourceMan;
		}

		public ResourceManager ResourceManager { get; private set; }

		public string SiteName {
			get { return ResourceManager.GetString("SiteName"); }
		}

		public string SiteTitle {
			get { return ResourceManager.GetString("SiteTitle"); }
		}

	}

}
