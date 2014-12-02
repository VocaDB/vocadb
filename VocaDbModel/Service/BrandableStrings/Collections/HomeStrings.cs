using System.Resources;

namespace VocaDb.Model.Service.BrandableStrings.Collections {

	public class HomeStrings {

		public HomeStrings(ResourceManager resourceMan) {
			ResourceManager = resourceMan;
		}

		public ResourceManager ResourceManager { get; private set; }

		public string SiteDescription {
			get {
				return ResourceManager.GetString("SiteDescription");
			}
		}

		public string Welcome {
			get {
				return ResourceManager.GetString("Welcome");
			}
		}

		public string WelcomeSubtitle {
			get {
				return ResourceManager.GetString("WelcomeSubtitle");
			}
		}

	}

}
