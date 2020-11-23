using System.Configuration;
using System.Linq;
using System.Xml;

namespace VocaDb.Model.Utils.Config
{
	public class GlobalLinksSection : ConfigurationSectionGroup
	{
		public LinksSection AppLinks { get; set; }

		public LinksSection BigBanners { get; set; }

		public LinksSection SmallBanners { get; set; }

		public LinksSection SocialSites { get; set; }
	}

	public class LinksSection : ConfigurationSection
	{
		public LinkElement[] Links { get; set; }
	}

	public class LinksSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			var links = section.ChildNodes.Cast<XmlNode>().Select(n => new LinkElement(
				n.Attributes["bannerImg"].Value, n.Attributes["title"].Value, n.Attributes["url"].Value));

			return new LinksSection { Links = links.ToArray() };
		}
	}
	public class LinkElement
	{
		public LinkElement(string bannerImg, string title, string url)
		{
			BannerImg = bannerImg;
			Title = title;
			Url = url;
		}

		public string BannerImg { get; set; }

		public string Title { get; set; }

		public string Url { get; set; }
	}
}
