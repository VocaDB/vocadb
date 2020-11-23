using System.Resources;
using VocaDb.Model.Resources.Views;

namespace VocaDb.Model.Service.BrandableStrings.Collections
{
	public class LayoutStrings : ResStringCollection
	{
		public LayoutStrings(ResourceManager resourceMan) : base(resourceMan) { }

		public string Keywords => GetString(nameof(LayoutRes.Keywords));
		public string PaypalDonateTitle => GetString(nameof(LayoutRes.PaypalDonateTitle));
		public string SiteName => GetString(nameof(LayoutRes.SiteName));
		public string SiteTitle => GetString(nameof(LayoutRes.SiteTitle));
	}
}
