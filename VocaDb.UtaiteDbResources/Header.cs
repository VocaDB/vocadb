using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Service.BrandableStrings.Collections;
using VocaDb.UtaiteDb.Resources.ViewRes;

namespace VocaDb.UtaiteDb.Resources {

	public class Header : IBrandedStringsAssemblyHeader {

		public AlbumStrings Album {
			get {
				return new AlbumStrings(AlbumRes.ResourceManager);
			}
		}

		public ArtistStrings Artist {
			get {
				return new ArtistStrings(ArtistRes.ResourceManager);
			}
		}

		public HomeStrings Home {
			get {
				return new HomeStrings(HomeRes.ResourceManager);
			}
		}

		public LayoutStrings Layout {
			get {
				return new LayoutStrings(LayoutRes.ResourceManager);
			}
		}

		public SongStrings Song {
			get {
				return new SongStrings(SongRes.ResourceManager);
			}
		}

	}
}
