using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;
using NLog;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices {

	public class VideoServiceBilibili : VideoService {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public static readonly RegexLinkMatcher[] Matchers =
			{
				new RegexLinkMatcher("acg.tv/av{0}", @"www.bilibili.com/video/av(\d+)"),
				new RegexLinkMatcher("acg.tv/av{0}", @"acg.tv/av(\d+)"),
				new RegexLinkMatcher("acg.tv/av{0}", @"www.bilibili.tv/video/av(\d+)"),
				new RegexLinkMatcher("acg.tv/av{0}", @"bilibili.kankanews.com/video/av(\d+)")
			};

		public VideoServiceBilibili() 
			: base(PVService.Bilibili, null, Matchers) {}

		public override VideoUrlParseResult ParseByUrl(string url, bool getTitle) {

			var id = GetIdByUrl(url);

			if (string.IsNullOrEmpty(id))
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.NoMatcher, "No matcher");

			var paramStr = string.Format("appkey={0}&id={1}&type=xml{2}", AppConfig.BilibiliAppKey, id, AppConfig.BilibiliSecretKey);
			var paramStrMd5 = CryptoHelper.HashString(paramStr, CryptoHelper.MD5).ToLowerInvariant();

			var requestUrl = string.Format("https://api.bilibili.com/view?appkey={0}&id={1}&type=xml&sign={2}", AppConfig.BilibiliAppKey, id, paramStrMd5);

			var request = (HttpWebRequest)WebRequest.Create(requestUrl);
			request.UserAgent = "VocaDB/1.0 (admin@vocadb.net)";
			request.Timeout = 10000;
			XDocument doc;

			try {
				using (var response = request.GetResponse())
				using (var stream = response.GetResponseStream()) {
					doc = XDocument.Load(stream);
				}
			} catch (WebException x) {
				log.Warn(string.Format("Unable to load Bilibili URL {0}", url), x);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(string.Format("Unable to load Bilibili URL: {0}", x.Message), x));
			} catch (XmlException x) {
				log.Warn(string.Format("Unable to load Bilibili URL {0}", url), x);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(string.Format("Unable to load Bilibili URL: {0}", x.Message), x));
			}

			var titleElem = doc.XPathSelectElement("/info/title");
			var thumbElem = doc.XPathSelectElement("/info/pic");
			var authorElem = doc.XPathSelectElement("/info/author");

			if (titleElem == null)
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "No title element");

			var title = HtmlEntity.DeEntitize(titleElem.Value);
			var thumb = thumbElem != null ? thumbElem.Value : string.Empty;
			var author = authorElem != null ? authorElem.Value : string.Empty;

			return VideoUrlParseResult.CreateOk(url, PVService.Bilibili, id, 
				VideoTitleParseResult.CreateSuccess(title, author, thumb));

		}

	}
}
