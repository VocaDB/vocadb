using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Service.BrandableStrings.Collections;
using VocaDb.UtaiteDb.Resources.ViewRes;

namespace VocaDb.UtaiteDb.Resources
{
	public class Header : IBrandedStringsAssemblyHeader
	{
		public AlbumStrings Album => new AlbumStrings(AlbumRes.ResourceManager);

		public ArtistStrings Artist => new ArtistStrings(ArtistRes.ResourceManager);

		public HomeStrings Home => new HomeStrings(HomeRes.ResourceManager);

		public LayoutStrings Layout => new LayoutStrings(LayoutRes.ResourceManager);

		public SongStrings Song => new SongStrings(SongRes.ResourceManager);

		public UserStrings User => new UserStrings(UserRes.ResourceManager);
	}
}
