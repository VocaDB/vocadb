using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NLog;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Model.Service.AlbumImport {

	public class KarenTAlbumImporter : IAlbumImporter {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private static readonly RegexLinkMatcher matcher = new RegexLinkMatcher("https://karent.jp/album/{0}", @"http(?:s?)://karent.jp/album/(\d+)");
		private readonly IPictureDownloader pictureDownloader;

		public KarenTAlbumImporter(IPictureDownloader pictureDownloader) {
			this.pictureDownloader = pictureDownloader;
		}

		private PictureDataContract DownloadCoverPicture(string url) {

			if (url.Contains("_0280_0280.jpg")) {

				var fullUrl = url.Replace("_0280_0280", string.Empty);

				var pic = pictureDownloader.Create(fullUrl);

				if (pic != null)
					return pic;

			}

			return pictureDownloader.Create(url);

		}

		public MikuDbAlbumContract GetAlbumData(HtmlDocument doc, VocaDbUrl url) {

			var data = new ImportedAlbumDataContract();

			var titleElem = doc.DocumentNode.SelectSingleNode("//div[@class = 'pgtitle_in']/h1/span");

			if (titleElem != null)
				data.Title = HtmlEntity.DeEntitize(titleElem.InnerText);

			var mainPanel = doc.GetElementbyId("main_ref");

			if (mainPanel != null) {

				var descBox = mainPanel.SelectSingleNode("p[@class = 'overview']");

				if (descBox != null)
					data.Description = descBox.InnerText;

				var infoBox = mainPanel.SelectSingleNode("div[1]");

				if (infoBox != null)
					ParseInfoBox(data, infoBox);

				var tracklistElem = mainPanel.SelectSingleNode("div[@class = 'songlistbox']");

				if (tracklistElem != null)
					ParseTracklist(data, tracklistElem);

			}

			var coverElem = doc.DocumentNode.SelectSingleNode("//div[@id = 'sub_ref']/div[@class = 'artwork']/div/a/img");
			PictureDataContract coverPic = null;

			if (coverElem != null) {
				coverPic = DownloadCoverPicture("https://karent.jp" + coverElem.Attributes["src"].Value);
			}

			return new MikuDbAlbumContract(data) { CoverPicture = coverPic, SourceUrl = url.Url };

		}

		private string GetVocalistName(string portraitImg) {
			
			switch (portraitImg) {
				case "/modpub/images/ico/ico_cv_1.png":
					return "Hatsune Miku";
				case "/modpub/images/ico/ico_cv_2.png":
					return "Kagamine Rin";
				case "/modpub/images/ico/ico_cv_3.png":
					return "Kagamine Len";
				case "/modpub/images/ico/ico_cv_4.png":
					return "Megurine Luka";
				case "/modpub/images/ico/ico_cv_5.png":
					return "MEIKO";
				case "/modpub/images/ico/ico_cv_6.png":
					return "KAITO";
				default:
					return null;
			}

		}

		private HtmlNode GetInfoElem(HtmlNodeCollection nodes, string title) {
			return nodes.FirstOrDefault(n => n.SelectNodes("span[text() = '" + title + "&nbsp;:']") != null);
		}

		private void ParseInfoBox(ImportedAlbumDataContract data, HtmlNode infoBox) {

			var statusRows = infoBox.SelectNodes("//p[@class='albumstatus']");

			var artistRow = GetInfoElem(statusRows, "Artist");

			if (artistRow != null) {

				var links = artistRow.SelectNodes("a");
				data.ArtistNames = links.Select(l => l.InnerText).ToArray();

			}

			var releaseDateRow = GetInfoElem(statusRows, "Release");

			if (releaseDateRow != null) {

				DateTime releaseDate;
				if (DateTime.TryParseExact(releaseDateRow.Element("#text").InnerText, "yyyy.MM.dd", null, DateTimeStyles.None, out releaseDate))
					data.ReleaseYear = releaseDate.Year;

			}

			var charaRow = GetInfoElem(statusRows, "Characters");
			
			if (charaRow != null) {

				var charaImgs = charaRow.SelectNodes("a/img");
				data.VocalistNames = charaImgs.Select(l => GetVocalistName(l.Attributes["src"].Value)).Where(l => l != null).ToArray();

			}

		}

		public ImportedAlbumTrack ParseTrackRow(int trackNum, string songTitle) {

			var trackRegex = new Regex(@"\d\d\.\&nbsp\;(.+) (?:(?:\(feat\. (.+)\))|(\-\s?off vocal))"); // 01.&nbsp;Cloud Science (feat. Hatsune Miku)

			var match = trackRegex.Match(songTitle);

			if (!match.Success)
				return null;

			var title = HtmlEntity.DeEntitize(match.Groups[1].Value);
			string[] vocalists;

			if (match.Groups[2].Value == "- off vocal") {
				// TODO: set song type to instrumental
				vocalists = new string[0];
			} else {
				vocalists = match.Groups[2].Value.Split(new[] {'&'}, StringSplitOptions.RemoveEmptyEntries);
			}

			return new ImportedAlbumTrack { TrackNum = trackNum, DiscNum = 1, Title = title, VocalistNames = vocalists };

		}

		private void ParseTracklist(ImportedAlbumDataContract data, HtmlNode tracklistElem) {

			var songElems = tracklistElem.SelectNodes("//div[@class = 'song']");

			var tracks = new List<ImportedAlbumTrack>();
			for (int i = 1; i <= songElems.Count; ++i) {

				var songLink = songElems[i-1].Element("a");
				var track = ParseTrackRow(i, songLink.InnerText);

				if (track != null)
					tracks.Add(track);

			}

			data.Tracks = tracks.ToArray();

		}

		public AlbumImportResult ImportOne(VocaDbUrl url) {

			HtmlDocument doc;

			try {
				doc = HtmlRequestHelper.Download(url.Url, "en-US");
			} catch (WebException x) {
				log.Warn("Unable to download album post '" + url + "'", x);
				throw;
			}

			var data = GetAlbumData(doc, url);

			return new AlbumImportResult {AlbumContract = data};

		}

		public bool IsValidFor(VocaDbUrl url) => matcher.IsMatch(url);

		public string ServiceName {
			get { return "KarenT"; }
		}

		public override string ToString() {
			return "Album importer for KarenT";
		}

	}

}
