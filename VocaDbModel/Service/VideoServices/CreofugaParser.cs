using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.VideoServices {

	public class CreofugaParser : IVideoServiceParser {

		private int? ParseLength(string lengthStr) {

			if (string.IsNullOrEmpty(lengthStr))
				return null;

			TimeSpan timespan;
			if (TimeSpan.TryParseExact(lengthStr, "mm\\:ss", CultureInfo.InvariantCulture, out timespan)) {
				return (int)timespan.TotalSeconds;
			}

			return null;

		}

		private DateTime? ParseDate(string dateStr) {

			if (string.IsNullOrEmpty(dateStr))
				return null;

			DateTime date;
			return DateTime.TryParseExact(dateStr, "yy/MM/dd mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out date) ? (DateTime?)date : null; // 15/10/15 21:20

		}

		private VideoTitleParseResult ParseDocument(HtmlDocument doc, string url) {

			var title = doc.DocumentNode.SelectSingleNode("//meta[@name = 'twitter:title']")?.Attributes["content"]?.Value;
			title = !string.IsNullOrEmpty(title) ? title.Substring(0, title.Length - 1) : title;
			var thumb = doc.DocumentNode.SelectSingleNode("//meta[@name = 'twitter:image']")?.Attributes["content"]?.Value;
			var length = ParseLength(doc.DocumentNode.SelectSingleNode("//p[contains(@class, 'dummy_current_time_label')]")?.InnerText.Trim());
			var date = ParseDate(doc.DocumentNode.SelectSingleNode("//div[@class = 'audio-main-content-info-heading']")?.InnerText);
			var author = doc.DocumentNode.SelectSingleNode("//a[@class = 'user-info-icon']")?.Attributes["title"]?.Value; // <a class="user-info-icon" title="ERIGON" href="/erigon">

			return VideoTitleParseResult.CreateSuccess(title, author, null, thumb, length, uploadDate: date);

		}

		private VideoTitleParseResult ParseByHtmlStream(Stream htmlStream, Encoding encoding, string url) {
			var doc = new HtmlDocument();
			doc.Load(htmlStream, encoding);
			return this.ParseDocument(doc, url);
		}

		public Task<VideoTitleParseResult> GetTitleAsync(string id) {
			var url = string.Format("https://creofuga.net/audios/{0}", id);
			return HtmlRequestHelper.GetStreamAsync(url, stream => ParseByHtmlStream(stream, Encoding.UTF8, url));
		}

	}

}
