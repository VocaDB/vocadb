using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using VocaDb.SiteMapGenerator.VocaDb;

namespace VocaDb.SiteMapGenerator.Sitemap
{

	public class SitemapGenerator
	{

		private const string ns_sitemap = "http://www.sitemaps.org/schemas/sitemap/0.9";
		private const int maxEntriesPerSitemap = 50000;
		private readonly string sitemapRootUrl;
		private readonly string siteRoot;

		private XElement CreateUrlElement(EntryType entryType, EntryReference id)
		{

			return new XElement(XName.Get("url", ns_sitemap),
				 new XElement(XName.Get("loc", ns_sitemap), GenerateEntryUrl(entryType, id))
			);

		}

		private string GenerateEntryUrl(EntryType entryType, EntryReference id)
		{

			switch (entryType)
			{
				case EntryType.Album:
					return string.Format("{0}Al/{1}", siteRoot, id.Id);
				case EntryType.Artist:
					return string.Format("{0}Ar/{1}", siteRoot, id.Id);
				case EntryType.ReleaseEvent:
					return string.Format("{0}E/{1}/{2}", siteRoot, id.Id, id.UrlSlug);
				case EntryType.Song:
					return string.Format("{0}S/{1}", siteRoot, id.Id);
				case EntryType.Tag:
					return string.Format("{0}T/{1}/{2}", siteRoot, id.Id, id.UrlSlug);
			}

			return string.Empty;

		}

		private IEnumerable<XElement> CreateUrlElements(Dictionary<EntryType, IEnumerable<EntryReference>> entries)
		{

			var elements =
				(from entryType in entries.Keys
				 from entryId in entries[entryType]
				 select CreateUrlElement(entryType, entryId));

			return elements;

		}

		public SitemapGenerator(string siteRoot, string sitemapRootUrl)
		{
			this.siteRoot = siteRoot;
			this.sitemapRootUrl = sitemapRootUrl;
		}

		public void Generate(string outFolder, Dictionary<EntryType, IEnumerable<EntryReference>> entries)
		{

			var indexDoc = new XDocument(
				new XDeclaration("1.0", "UTF-8", "yes"),
				new XElement(XName.Get("sitemapindex", ns_sitemap)));

			var totalEntries = entries.Sum(e => e.Value.Count());
			var allUrlElements = CreateUrlElements(entries);
			var sitemapCount = Math.Ceiling(totalEntries / (double)maxEntriesPerSitemap);

			for (int sitemapNumber = 1; sitemapNumber <= sitemapCount; ++sitemapNumber)
			{

				var sitemapDoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
					new XElement(XName.Get("urlset", ns_sitemap)));

				var begin = (sitemapNumber - 1) * maxEntriesPerSitemap;
				var sitemapElements = allUrlElements.Skip(begin).Take(maxEntriesPerSitemap);

				foreach (var element in sitemapElements)
				{
					sitemapDoc.Root.Add(element);
				}

				var sitemapFile = string.Format("sitemap-{0}.xml", sitemapNumber);
				var sitemapPath = Path.Combine(outFolder, sitemapFile);
				var sitemapUrl = sitemapRootUrl + sitemapFile;
				sitemapDoc.Save(sitemapPath);

				var sitemapReferenceElement = new XElement(XName.Get("sitemap", ns_sitemap),
					new XElement(XName.Get("loc", ns_sitemap), sitemapUrl)
				);

				indexDoc.Root.Add(sitemapReferenceElement);

			}

			var indexOutPath = Path.Combine(outFolder, "sitemap-index.xml");
			indexDoc.Save(indexOutPath);

		}

	}

}
