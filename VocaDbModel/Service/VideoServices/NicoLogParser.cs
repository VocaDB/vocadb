using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NLog;

namespace VocaDb.Model.Service.VideoServices {
	public class NicoLogParser : IVideoServiceParser {
		public Task<VideoTitleParseResult> GetTitleAsync(string id) => NicoLogHelper.GetVideoTitleParseResultAsync(id);
	}

	public static class NicoLogHelper {
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static HtmlDocument GetVideoHtmlPage(string videoId) {
			var url = string.Format("https://nicolog.jp/watch/{0}", videoId);

			var request = WebRequest.Create(url);
			request.Timeout = 10000;
			WebResponse response;

			try {
				response = request.GetResponse(); // Closed below
			} catch (WebException x) {
				log.Warn(x, "Unable to get response for nicolog page");
				return null;
			}

			try {
				using (var stream = response.GetResponseStream()) {
					var doc = new HtmlDocument();
					try {
						doc.Load(stream, Encoding.UTF8);
					} catch (IOException x) {
						log.Warn(x, "Unable to load document for nicolog page");
					}

					return doc;
				}
			} finally {
				response.Close();
			}
		}

		public static Task<VideoTitleParseResult> GetVideoTitleParseResultAsync(string id) {
			HtmlDocument doc = GetVideoHtmlPage(id);
			VideoTitleParseResult videoTitleParseResult = ParsePage(doc);
			
			return Task.FromResult(videoTitleParseResult);
		}

		public static VideoTitleParseResult ParsePage(HtmlDocument doc) {
			var tagsNode = doc.DocumentNode.SelectSingleNode("//ul[@class='list-inline']");
			if (tagsNode == null) {
				return VideoTitleParseResult.CreateError("NicoLog (error): no info about video");
			}

			var metaTable = doc.DocumentNode.SelectSingleNode("//dl[@class='dl-horizontal']");
			String title = metaTable.SelectSingleNode("//dd[2]").InnerText;
			var replace = metaTable.SelectSingleNode("//dd[3]").InnerText.Replace("年", ".").Replace("月", ".").Replace("日 ", " ").Replace("時", ":").Replace("分", ":").Replace("秒", "");
			DateTime uploadDate = DateTime.ParseExact(replace, "yyyy.M.d H:mm:ss", CultureInfo.InvariantCulture);
			int lengthSeconds = (int) TimeSpan.ParseExact(metaTable.SelectSingleNode("//dd[4]").InnerText, "h\\:mm\\:ss", CultureInfo.InvariantCulture).TotalSeconds;
			String author = Regex.Replace(metaTable.SelectSingleNode("//dd[5]").InnerText, @"(\s\(ID:\d+\))","");
			String authorId = metaTable.SelectSingleNode("//dd[5]/a").GetAttributeValue("href", "").Split('/')[1];
			String thumbUrl = doc.DocumentNode.SelectSingleNode("//img[@class='center-block img-thumbnail']").GetAttributeValue("src","");

			var result = VideoTitleParseResult.CreateSuccess(title, author, authorId, thumbUrl,
				lengthSeconds, uploadDate: uploadDate);
			var tags = tagsNode.ChildNodes.Select(node => node.InnerText).ToArray();
			result.Tags = tags;

			return result;
		}
	}
}