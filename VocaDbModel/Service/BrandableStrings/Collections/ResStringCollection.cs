using System.Resources;

namespace VocaDb.Model.Service.BrandableStrings.Collections {

	public abstract class ResStringCollection {

		protected ResStringCollection(ResourceManager resourceManager) {
			ResourceManager = resourceManager;
		}

		public ResourceManager ResourceManager { get; private set; }

		protected string GetString(string name) {
			return ResourceManager.GetString(name);
		}

	}

}
