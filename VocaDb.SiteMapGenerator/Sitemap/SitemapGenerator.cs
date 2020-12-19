#nullable disable

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
		private const string Ns_sitemap = "http://www.sitemaps.org/schemas/sitemap/0.9";
		private const int MaxEntriesPerSitemap = 50000;
		private readonly string _sitemapRootUrl;
		private readonly string _siteRoot;

		private XElement CreateUrlElement(EntryType entryType, EntryReference id)
		{
			return new XElement(XName.Get("url", Ns_sitemap),
				 new XElement(XName.Get("loc", Ns_sitemap), GenerateEntryUrl(entryType, id))
			);
		}

		private string GenerateEntryUrl(EntryType entryType, EntryReference id) => entryType switch
		{
			EntryType.Album => $"{_siteRoot}Al/{id.Id}",
			EntryType.Artist => $"{_siteRoot}Ar/{id.Id}",
			EntryType.ReleaseEvent => $"{_siteRoot}E/{id.Id}/{id.UrlSlug}",
			EntryType.Song => $"{_siteRoot}S/{id.Id}",
			EntryType.Tag => $"{_siteRoot}T/{id.Id}/{id.UrlSlug}",
			_ => string.Empty,
		};

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
			this._siteRoot = siteRoot;
			this._sitemapRootUrl = sitemapRootUrl;
		}

		public void Generate(string outFolder, Dictionary<EntryType, IEnumerable<EntryReference>> entries)
		{
			var indexDoc = new XDocument(
				new XDeclaration("1.0", "UTF-8", "yes"),
				new XElement(XName.Get("sitemapindex", Ns_sitemap)));

			var totalEntries = entries.Sum(e => e.Value.Count());
			var allUrlElements = CreateUrlElements(entries);
			var sitemapCount = Math.Ceiling(totalEntries / (double)MaxEntriesPerSitemap);

			for (int sitemapNumber = 1; sitemapNumber <= sitemapCount; ++sitemapNumber)
			{
				var sitemapDoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
					new XElement(XName.Get("urlset", Ns_sitemap)));

				var begin = (sitemapNumber - 1) * MaxEntriesPerSitemap;
				var sitemapElements = allUrlElements.Skip(begin).Take(MaxEntriesPerSitemap);

				foreach (var element in sitemapElements)
				{
					sitemapDoc.Root.Add(element);
				}

				var sitemapFile = $"sitemap-{sitemapNumber}.xml";
				var sitemapPath = Path.Combine(outFolder, sitemapFile);
				var sitemapUrl = _sitemapRootUrl + sitemapFile;
				sitemapDoc.Save(sitemapPath);

				var sitemapReferenceElement = new XElement(XName.Get("sitemap", Ns_sitemap),
					new XElement(XName.Get("loc", Ns_sitemap), sitemapUrl)
				);

				indexDoc.Root.Add(sitemapReferenceElement);
			}

			var indexOutPath = Path.Combine(outFolder, "sitemap-index.xml");
			indexDoc.Save(indexOutPath);
		}
	}
}
