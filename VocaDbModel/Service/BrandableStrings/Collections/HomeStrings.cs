using System.Resources;
using VocaDb.Model.Resources.Views;

namespace VocaDb.Model.Service.BrandableStrings.Collections
{

	public class HomeStrings : ResStringCollection
	{

		public HomeStrings(ResourceManager resourceMan) : base(resourceMan) { }

		public string SiteDescription => GetString(nameof(HomeRes.SiteDescription));
		public string Welcome => GetString(nameof(HomeRes.Welcome));
		public string WelcomeSubtitle => GetString(nameof(HomeRes.WelcomeSubtitle));

	}

}
