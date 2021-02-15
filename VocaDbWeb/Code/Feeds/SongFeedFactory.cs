#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code.Feeds
{
	public class SongFeedFactory
	{
		private static readonly XNamespace s_mediaNs = XNamespace.Get(@"http://search.yahoo.com/mrss/");

		private async Task<SyndicationItem> CreateFeedItemAsync(SongContract song, Func<SongContract, Task<string>> contentFac, Func<SongContract, string> urlFac)
		{
			var item = new SyndicationItem(song.Name, new TextSyndicationContent(await contentFac(song), TextSyndicationContentKind.Html),
					VocaUriBuilder.CreateAbsolute(urlFac(song)), song.Id.ToString(), song.CreateDate);

			item.Summary = new TextSyndicationContent(await contentFac(song), TextSyndicationContentKind.Html);
			if (!string.IsNullOrEmpty(song.ThumbUrl))
				item.ElementExtensions.Add(new XElement(s_mediaNs + "thumbnail", new XAttribute("url", song.ThumbUrl)));

			return item;
		}

		public async Task<SyndicationFeed> CreateAsync(IEnumerable<SongContract> songs, Uri uri, Func<SongContract, Task<string>> contentFac, Func<SongContract, string> urlFac)
		{
			var items = await Task.WhenAll(songs.Select(s => CreateFeedItemAsync(s, contentFac, urlFac)));

			var feed = new SyndicationFeed("Latest songs with videos", string.Empty, uri, items);
			feed.AttributeExtensions.Add(new XmlQualifiedName("media", XNamespace.Xmlns.ToString()), s_mediaNs.ToString());

			return feed;
		}
	}
}