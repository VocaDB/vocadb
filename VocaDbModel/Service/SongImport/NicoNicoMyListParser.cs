using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NLog;
using Rss;
using VocaDb.Model.DataContracts.SongImport;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.SongImport
{

	public class NicoNicoMyListParser : ISongListImporter
	{

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static readonly Regex wvrIdRegex = new Regex(@"#(\d{3})");

		public Task<PartialImportedSongs> GetSongsAsync(string url, string nextPageToken, int maxResults, bool parseAll)
		{
			throw new NotSupportedException();
		}

		private bool IsRankingsItem(RssItem item)
		{
			var node = HtmlNode.CreateNode($"<div>{item.Description}</div>");
			return node.InnerText.Any() && char.IsDigit(node.InnerText, 0);
		}

		public Task<ImportedSongListContract> ParseAsync(string url, bool parseAll)
		{

			if (string.IsNullOrEmpty(url))
				throw new UnableToImportException("Feed URL cannot be empty");

			if (!url.Contains("rss="))
				url += "?rss=2.0";

			RssFeed feed;

			try
			{
				feed = RssFeed.Read(url);
			}
			catch (UriFormatException x)
			{
				log.Warn(x, "Unable to parse URL");
				throw new UnableToImportException("Unable to parse URL", x);
			}
			catch (WebException x)
			{
				log.Error(x, "Unable to parse feed");
				throw new UnableToImportException("Unable to parse feed", x);
			}

			if (feed.Exceptions.LastException != null)
			{
				log.Error(feed.Exceptions.LastException, "Unable to parse feed");
				throw new UnableToImportException("Unable to parse feed", feed.Exceptions.LastException);
			}

			var result = new ImportedSongListContract();
			var channel = feed.Channels[0];
			result.Name = channel.Title;
			var wvrIdMatch = wvrIdRegex.Match(result.Name);

			if (wvrIdMatch.Success)
				result.WVRNumber = int.Parse(wvrIdMatch.Groups[1].Value);

			var songs = new List<ImportedSongInListContract>();
			var order = 1;

			foreach (var item in channel.Items.Cast<RssItem>())
			{

				if (parseAll || IsRankingsItem(item))
				{

					var nicoId = VideoService.NicoNicoDouga.GetIdByUrl(item.Link.ToString());
					songs.Add(new ImportedSongInListContract(PVService.NicoNicoDouga, nicoId)
					{
						SortIndex = order,
						Name = item.Title,
						Url = item.Link.ToString()
					});
					++order;

				}

			}

			result.Songs = new PartialImportedSongs(songs.ToArray(), songs.Count, null);
			return Task.FromResult(result);

		}

		public bool MatchUrl(string url)
		{
			var regex = new Regex(@"www.nicovideo.jp/mylist/\d+");
			return regex.IsMatch(url);
		}

	}

}
