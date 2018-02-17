using System.Linq;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code {

	/// <summary>
	/// Left hand side menu
	/// </summary>
	public abstract class MenuPage : VocaDbPage {

		public class Link {

			public Link(string title, string url, string bannerImg) {
				BannerImg = bannerImg;
				Title = title;
				Url = url;
			}

			public string BannerImg { get; set; }

			public string Title { get; set; }

			public string Url { get; set; }

		}

		static MenuPage() {
			
			var config = AppConfig.GetGlobalLinksSection();

			if (config == null || config.AppLinks == null) {

				AppLinks = new[] {
					new Link("Google Play Store", "https://play.google.com/store/apps/details?id=com.coolappz.Vocadb", "en_app_rgb_wo_45.png"),
					new Link("App Store", "https://itunes.apple.com/us/app/vocadb/id907510673", "appstore.png"),
				};

			} else {
			
				AppLinks = config.AppLinks.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).ToArray();
	
			}

			if (config == null || config.BigBanners == null) {

				BigBanners = new Link[0];

			} else {
				
				BigBanners = config.BigBanners.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).RandomSort().ToArray();

			}

			if (config == null || config.SmallBanners == null) {

				SmallBanners = new[] {
					new Link("UtaiteDB", "https://utaitedb.net", "utaitedb_small.png"),
					new Link("Mikufan.com", "http://www.mikufan.com", "mikufan_small.png"),
					new Link("Project DIVA wiki", "http://projectdiva.wiki/", "pjd-wiki_small.png"),
					new Link("r/vocaloid", "https://www.reddit.com/r/vocaloid", "rvocaloid_small2.png"),
					new Link("Vocallective", "http://www.vocallective.net", "vocallective_small.jpg"),
					new Link("Vocaloid News Network", "https://www.vocaloidnews.net/", "vnn.png"),
					new Link("VocaloidOtaku", "http://vocaloidotaku.net", "vo_small.png"),
					new Link("VocaEuro", "https://vocaeuro.wordpress.com/", "vocaeuro.jpg"),
				}.RandomSort().ToArray();

			} else {
				
				SmallBanners = config.SmallBanners.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).RandomSort().ToArray();

			}

			if (config == null || config.SocialSites == null) {

				SocialLinks = new[] {
					new Link("Facebook", "https://www.facebook.com/vocadb", "facebook.png"),
					new Link("Twitter", "https://twitter.com/VocaDB", "Twitter_Logo.png"),
					new Link("Google+", "https://plus.google.com/112203842561345720098", "googleplus-icon.png")				
				};

			} else {
			
				SocialLinks = config.SocialSites.Links.Select(l => new Link(l.Title, l.Url, l.BannerImg)).ToArray();
	
			}

		}

		public static Link[] AppLinks { get; private set; }

		public static Link[] BigBanners { get; private set; }

		public string BlogUrl => UrlHelper.MakeLink(Config.SiteSettings.BlogUrl);

		public static Link[] SmallBanners { get; private set; }

		public static Link[] SocialLinks { get; private set; }

	}

}