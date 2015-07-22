using System.Resources;

namespace VocaDb.Model.Service.BrandableStrings.Collections {

	public class ArtistStrings {

		public ArtistStrings(ResourceManager resourceMan) {
			ResourceManager = resourceMan;
		}

		public ResourceManager ResourceManager { get; }

		public string AuthoredBy => ResourceManager.GetString("AuthoredBy");

		public string NewArtistExternalLink => ResourceManager.GetString("NewArtistExternalLink");
	}

}
