using System.Resources;
using VocaDb.Model.Resources.Views;

namespace VocaDb.Model.Service.BrandableStrings.Collections
{

	public class AlbumStrings : ResStringCollection
	{

		public AlbumStrings(ResourceManager resourceMan) : base(resourceMan) { }

		public string NewAlbumArtistDesc => GetString(nameof(AlbumRes.NewAlbumArtistDesc));
		public string NewAlbumInfo => GetString(nameof(AlbumRes.NewAlbumInfo));

	}

}
