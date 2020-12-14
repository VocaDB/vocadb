#nullable disable

using System.Resources;
using VocaDb.Model.Resources.Views;

namespace VocaDb.Model.Service.BrandableStrings.Collections
{
	public class ArtistStrings : ResStringCollection
	{
		public ArtistStrings(ResourceManager resourceMan) : base(resourceMan) { }

		public string AuthoredBy => GetString(nameof(ArtistRes.AuthoredBy));
		public string NewArtistExternalLink => GetString(nameof(ArtistRes.NewArtistExternalLink));
	}
}
