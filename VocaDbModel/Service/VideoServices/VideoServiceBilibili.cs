using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;
using Newtonsoft.Json;
using NLog;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;
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

		private string GetValue(XDocument doc, string xpath) {
			return doc.XPathSelectElement(xpath)?.Value;
		}

		private async Task<int?> GetLength(string id) {

			var requestUrl = string.Format("https://api.bilibili.com/x/player/pagelist?aid={0}", id);

			PlayerResponse result;

			try {
				result = await JsonRequest.ReadObjectAsync<PlayerResponse>(requestUrl);
			} catch (WebException) {
				return null;
			} catch (JsonSerializationException) {
				return null;
			}

			return result?.Data.FirstOrDefault()?.Duration;

		}

		public override async Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle) {

			var id = GetIdByUrl(url);

			if (string.IsNullOrEmpty(id))
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.NoMatcher, "No matcher");

			if (!getTitle) {
				return VideoUrlParseResult.CreateOk(url, PVService.Bilibili, id, VideoTitleParseResult.Empty);
			}

			var paramStr = string.Format("appkey={0}&id={1}&type=xml{2}", AppConfig.BilibiliAppKey, id, AppConfig.BilibiliSecretKey);
			var paramStrMd5 = CryptoHelper.HashString(paramStr, CryptoHelper.MD5).ToLowerInvariant();

			var requestUrl = string.Format("https://api.bilibili.com/view?appkey={0}&id={1}&type=xml&sign={2}", AppConfig.BilibiliAppKey, id, paramStrMd5);

			XDocument doc;

			try {
				doc = await HtmlRequestHelper.GetStreamAsync(requestUrl, stream => XDocument.Load(stream), timeoutSec: 10, userAgent: "VocaDB/1.0 (admin@vocadb.net)");
			} catch (WebException x) {
				log.Warn(x, "Unable to load Bilibili URL {0}", url);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(string.Format("Unable to load Bilibili URL: {0}", x.Message), x));
			} catch (XmlException x) {
				log.Warn(x, "Unable to load Bilibili URL {0}", url);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(string.Format("Unable to load Bilibili URL: {0}", x.Message), x));
			} catch (IOException x) {
				log.Warn(x, "Unable to load Bilibili URL {0}", url);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(string.Format("Unable to load Bilibili URL: {0}", x.Message), x));
			}

			var titleElem = doc.XPathSelectElement("/info/title");
			var thumbElem = doc.XPathSelectElement("/info/pic");
			var authorElem = doc.XPathSelectElement("/info/author");
			var authorId = GetValue(doc, "/info/mid");
			var createdElem = doc.XPathSelectElement("/info/created_at");
			int.TryParse(GetValue(doc, "/info/cid"), out var cid); // Unsure what this is, but it's required for embedding

			if (titleElem == null)
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "No title element");

			var title = HtmlEntity.DeEntitize(titleElem.Value);
			var thumb = thumbElem?.Value ?? string.Empty;
			var author = authorElem?.Value ?? string.Empty;
			var created = createdElem != null ? (DateTime?)DateTime.Parse(createdElem.Value) : null;
			var length = await GetLength(id);

			var metadata = new PVExtendedMetadata(new BiliMetadata {
				Cid = cid
			});

			return VideoUrlParseResult.CreateOk(url, PVService.Bilibili, id, 
				VideoTitleParseResult.CreateSuccess(title, author, authorId, thumb, length: length, uploadDate: created, extendedMetadata: metadata));

		}

		public override IEnumerable<string> GetUserProfileUrls(string authorId) {
			return new[] {
				string.Format("http://space.bilibili.com/{0}", authorId),
				string.Format("http://space.bilibili.com/{0}/#!/index", authorId)
			};
		}

		public override string GetUrlById(string id, PVExtendedMetadata _) => $"https://www.bilibili.com/video/av{id}";

	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class BiliMetadata {
		[DataMember]
		public int Cid { get; set; }
	}

	class PlayerResponse {
		public PlayerResponseData[] Data { get; set; }
	}

	class PlayerResponseData {
		public int Duration { get; set; }
	}

}
