using System.Resources;

namespace VocaDb.Model.Service.BrandableStrings.Collections {

	public class LayoutStrings {

		public LayoutStrings(ResourceManager resourceMan) {
			ResourceManager = resourceMan;
		}

		public ResourceManager ResourceManager { get; }

		public string Keywords => ResourceManager.GetString("Keywords");

		public string PaypalDonateTitle => ResourceManager.GetString("PaypalDonateTitle");

		public string SiteName => ResourceManager.GetString("SiteName");

		public string SiteTitle => ResourceManager.GetString("SiteTitle");

	}

}
