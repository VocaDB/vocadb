using System.Resources;

namespace VocaDb.Model.Service.BrandableStrings.Collections {

	public class ArtistStrings {

		public ArtistStrings(ResourceManager resourceMan) {
			ResourceManager = resourceMan;
		}

		public ResourceManager ResourceManager { get; private set; }

		public string NewArtistExternalLink {
			get { return ResourceManager.GetString("NewArtistExternalLink"); }
		}

	}

}
