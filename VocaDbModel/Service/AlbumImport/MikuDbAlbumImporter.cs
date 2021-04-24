using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using HtmlAgilityPack;
using NLog;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.AlbumImport
{
	/// <summary>
	/// Imports albums from MikuDB.com (now dead).
	/// </summary>
	public class MikuDbAlbumImporter : IAlbumImporter
	{
		private const string AlbumIndexUrl = "http://mikudb.com/album-index/";
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		private const int MaxResults = 5;

		private readonly HashSet<string> _existingUrls;

		private bool ContainsTracklist(HtmlNode node)
		{
			var text = StripHtml(node.InnerText);

			return (LineMatch(text, "Track list") || LineMatch(text, "Tracks list"));
		}

		private HtmlNode? FindTracklistRow(HtmlDocument doc, HtmlNode? row)
		{
			// Find the first table row on the page
			if (row == null)
				row = doc.DocumentNode.SelectSingleNode(".//div[@class='postcontent']/table/tr[1]");

			if (row != null)
			{
				while (row != null)
				{
					var cell = row.Element("td");

					if (cell != null && ContainsTracklist(cell))
						return row;

					row = row.NextSibling;
				}
			}
			else
			{
				// Legacy pages don't have a <table>, but <p> elements instead
				row = doc.DocumentNode.SelectSingleNode(".//div[@class='postcontent']/p[2]");

				while (row != null)
				{
					if (ContainsTracklist(row))
						return row;

					row = row.NextSibling;
				}
			}

			return null;
		}

		private bool LineMatch(string line, string? field)
		{
			return line.StartsWith(field + ":") || line.StartsWith(field + " :");
		}

		private string ParseArtist(string artistName)
		{
			artistName = artistName.Trim();

			if (string.IsNullOrEmpty(artistName) || artistName == "-")
				return string.Empty;

			return artistName;
		}

		private string[] ParseArtists(string artistString)
		{
			return artistString
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.Where(s => s != "-")
				.ToArray();
		}

		private void ParseInfoBox(ImportedAlbumDataContract data, HtmlNode infoBox)
		{
			var text = infoBox.InnerHtml;
			var rows = text.Split(new[] { "<br>", "<br />", "\n" }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var row in rows)
			{
				var stripped = HtmlEntity.DeEntitize(StripHtml(row));
				if (stripped.StartsWith("\"") && stripped.EndsWith("\""))
					stripped = stripped.Substring(1, stripped.Length - 2).Trim();

				if (LineMatch(stripped, "Artist") || LineMatch(stripped, "Artists"))
				{
					var artists = ParseArtists(stripped.Substring(8));
					data.ArtistNames = artists;
				}
				else if (LineMatch(stripped, "Vocals"))
				{
					var vocals = ParseArtists(stripped.Substring(8));
					data.VocalistNames = vocals;
				}
				else if (LineMatch(stripped, "Circle"))
				{
					var artists = ParseArtist(stripped.Substring(8));
					data.CircleName = artists;
				}
				else if (LineMatch(stripped, "Year"))
				{
					if (int.TryParse(stripped.Substring(6), out int year))
						data.ReleaseYear = year;
				}
			}
		}

		private void ParseTrackList(ImportedAlbumDataContract data, HtmlNode cell)
		{
			var lines = cell.InnerText.Split(new[] { "<br>", "<br />", "\n" }, StringSplitOptions.RemoveEmptyEntries);

			int discNum = 1;
			var tracks = new List<ImportedAlbumTrack>();
			foreach (var line in lines)
			{
				var dotPos = line.IndexOf('.');

				if (dotPos <= 0)
					continue;

				var trackText = line.Substring(0, dotPos);

				if (int.TryParse(trackText, out int trackNum))
				{
					if (trackNum == 1 && tracks.Any())
						discNum++;

					var trackTitle = line.Substring(dotPos + 1, line.Length - dotPos - 1).Trim();
					trackTitle = trackTitle.Replace("(lyrics)", string.Empty);

					tracks.Add(new ImportedAlbumTrack
					{
						DiscNum = discNum,
						Title = HtmlEntity.DeEntitize(trackTitle),
						TrackNum = trackNum
					});
				}
			}

			data.Tracks = tracks.ToArray();
		}

		private string StripHtml(string html)
		{
			return HtmlHelperFunctions.StripHtml(html).Trim();
		}

		private MikuDbAlbumContract GetAlbumData(HtmlDocument doc, string url)
		{
			var data = new ImportedAlbumDataContract();

			string title = string.Empty;
			var titleElem = doc.DocumentNode.SelectSingleNode(".//h2[@class='posttitle']/a");

			if (titleElem != null)
				title = HtmlEntity.DeEntitize(titleElem.InnerText);

			var coverPicLink = doc.DocumentNode.SelectSingleNode(".//div[@class='postcontent']/table/tr[1]/td[1]/a/img");
			PictureDataContract? coverPicture = null;

			if (coverPicLink != null)
			{
				var address = coverPicLink.Attributes["src"].Value;

				coverPicture = DownloadCoverPicture(address);
			}

			var infoBox = doc.DocumentNode.SelectSingleNode(".//div[@class='postcontent']/table/tr[1]/td[2]");

			if (infoBox != null)
			{
				ParseInfoBox(data, infoBox);
			}

			var trackListRow = FindTracklistRow(doc, (infoBox != null ? infoBox.ParentNode.NextSibling : null));

			if (trackListRow != null)
			{
				ParseTrackList(data, trackListRow);
			}

			return new MikuDbAlbumContract { Title = title, Data = data, CoverPicture = coverPicture, SourceUrl = url };
		}

		private MikuDbAlbumContract GetAlbumData(string url)
		{
			HtmlDocument doc;

			try
			{
				doc = HtmlRequestHelper.Download(url);
			}
			catch (WebException x)
			{
				s_log.Warn("Unable to download album post '" + url + "'", x);
				throw;
			}

			return GetAlbumData(doc, url);
		}

		private AlbumImportResult[] Import(HtmlDocument doc)
		{
			var listDiv = doc.DocumentNode.SelectSingleNode("//div[@class = 'postcontent2']");
			var albumDivs = listDiv.Descendants("div");
			var list = new List<AlbumImportResult>();

			foreach (var albumDiv in albumDivs)
			{
				var link = albumDiv.Element("a");

				if (link == null)
					continue;

				var url = link.Attributes["href"].Value;

				if (_existingUrls.Contains(url))
					continue;

				//var name = HtmlEntity.DeEntitize(link.InnerText);
				var data = GetAlbumData(url);

				list.Add(new AlbumImportResult { AlbumContract = data });

				/*list.Add(new AlbumImportResult {
					AlbumContract = new MikuDbAlbumContract {
						Title = name, SourceUrl = url, CoverPicture = data.CoverPicture, Data = data.AlbumData
					}
				});*/

				if (list.Count >= MaxResults)
					break;

				Thread.Sleep(300);
			}

			return list.ToArray();
		}

		private PictureDataContract DownloadCoverPicture(string url)
		{
			WebRequest request;

			if (url.Contains("-250x250"))
			{
				var fullUrl = url.Replace("-250x250", string.Empty);

				request = WebRequest.Create(fullUrl);
				using (var response = (HttpWebResponse)request.GetResponse())
				{
					if (response.StatusCode != HttpStatusCode.NotFound)
					{
						using (var stream = response.GetResponseStream())
						{
							var buf = StreamHelper.ReadStream(stream, response.ContentLength);

							return new PictureDataContract(buf, response.ContentType);
						}
					}
				}
			}

			request = WebRequest.Create(url);
			using (var response = request.GetResponse())
			using (var stream = response.GetResponseStream())
			{
				var buf = StreamHelper.ReadStream(stream, response.ContentLength);

				return new PictureDataContract(buf, response.ContentType);
			}
		}

		public MikuDbAlbumImporter(IEnumerable<MikuDbAlbumContract> existingUrls)
		{
			ParamIs.NotNull(() => existingUrls);

			_existingUrls = new HashSet<string>(existingUrls.Select(a => a.SourceUrl));
		}

		public AlbumImportResult[] ImportNew()
		{
			HtmlDocument albumIndex;

			try
			{
				albumIndex = HtmlRequestHelper.Download(AlbumIndexUrl);
			}
			catch (WebException x)
			{
				s_log.Warn("Unable to read albums index", x);
				throw;
			}

			return Import(albumIndex);
		}

		public AlbumImportResult ImportOne(string url)
		{
			if (_existingUrls.Contains(url))
				return new AlbumImportResult { Message = "Album already imported" };

			var data = GetAlbumData(url);

			return new AlbumImportResult { AlbumContract = data };
		}

		public bool IsValidFor(string url)
		{
			return false;
		}

		public string ServiceName => "MikuDB";
	}
}
