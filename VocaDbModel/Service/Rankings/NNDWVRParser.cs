using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Rss;
using VocaDb.Model.DataContracts.Ranking;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.Rankings {

	public class NNDWVRParser {

		private static readonly Regex wvrIdRegex = new Regex(@"#(\d{3})");

		public RankingContract GetSongs(string url, bool parseAll) {

			if (string.IsNullOrEmpty(url))
				throw new InvalidFeedException("Feed URL cannot be empty");

			if (!url.Contains("rss="))
				url += "?rss=2.0";

			RssFeed feed;
			
			try {
				feed = RssFeed.Read(url);
			} catch (UriFormatException x) {
				throw new InvalidFeedException("Unable to parse URL", x);
			} catch (WebException x) {
				throw new InvalidFeedException("Unable to parse feed", x);				
			}

			if (feed.Exceptions.LastException != null) {
				throw new InvalidFeedException("Unable to parse feed", feed.Exceptions.LastException);
			}

			var result = new RankingContract();
			var channel = feed.Channels[0];
			result.Name = channel.Title;
			var wvrIdMatch = wvrIdRegex.Match(result.Name);

			if (wvrIdMatch.Success)
				result.WVRId = int.Parse(wvrIdMatch.Groups[1].Value);

			var songs = new List<SongInRankingContract>();
			var order = 1;

			foreach (var item in channel.Items.Cast<RssItem>()) {

				var node = HtmlNode.CreateNode(item.Description);

				if (parseAll || (node.InnerText.Any() && char.IsDigit(node.InnerText, 0))) {

					var nicoId = VideoService.NicoNicoDouga.GetIdByUrl(item.Link.ToString());
					songs.Add(new SongInRankingContract { NicoId = nicoId, SortIndex = order, Name = item.Title, Url = item.Link.ToString() });
					++order;

				}

			}

			result.Songs = songs.ToArray();
			return result;

		}

	}

}
