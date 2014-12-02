using System.Resources;

namespace VocaDb.Model.Service.BrandableStrings.Collections {

	public class AlbumStrings {

		public AlbumStrings(ResourceManager resourceMan) {
			ResourceManager = resourceMan;
		}

		public ResourceManager ResourceManager { get; private set; }

		public string NewAlbumArtistDesc {
			get { return ResourceManager.GetString("NewAlbumArtistDesc"); }
		}

		public string NewAlbumInfo {
			get { return ResourceManager.GetString("NewAlbumInfo"); }
		}

	}

}
