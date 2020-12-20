#nullable disable

using System.Linq;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code
{
	/// <summary>
	/// Left hand side menu
	/// </summary>
	public abstract class MenuPage : VocaDbPage
	{
		public class Link
		{
			public Link(string title, string url, string bannerImg)
			{
				BannerImg = bannerImg;
				Title = title;
				Url = url;
			}

			public string BannerImg { get; set; }

			public string Title { get; set; }

			public string Url { get; set; }
		}

		static MenuPage()
		{
			BannerUrl = AppConfig.SiteSettings.BannerUrl.EmptyToNull();

			var config = AppConfig.GetGlobalLinksSection();

			AppLinks = config?.AppLinks?.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).ToArray() ?? new Link[0];
			BigBanners = config?.BigBanners?.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).RandomSort().ToArray() ?? new Link[0];
			SmallBanners = config?.SmallBanners?.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).RandomSort().ToArray() ?? new Link[0];
			SocialLinks = config?.SocialSites?.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).ToArray() ?? new Link[0];
		}

		public static Link[] AppLinks { get; }

		/// <summary>
		/// Custom banner URL. Null if default.
		/// </summary>
		public static string BannerUrl { get; }

		public static Link[] BigBanners { get; }

		public string BlogUrl => UrlHelper.MakeLink(Config.SiteSettings.BlogUrl);

		public static Link[] SmallBanners { get; }

		public static Link[] SocialLinks { get; }
	}
}