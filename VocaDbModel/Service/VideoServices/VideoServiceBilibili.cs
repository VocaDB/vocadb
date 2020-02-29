using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
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

			var paramStr = string.Format("appkey={0}&id={1}&type=json{2}", AppConfig.BilibiliAppKey, id, AppConfig.BilibiliSecretKey);
			var paramStrMd5 = CryptoHelper.HashString(paramStr, CryptoHelper.MD5).ToLowerInvariant();

			var requestUrl = string.Format("https://api.bilibili.com/view?appkey={0}&id={1}&type=json&sign={2}", AppConfig.BilibiliAppKey, id, paramStrMd5);

			BilibiliResponse response;

			try {
				response = await JsonRequest.ReadObjectAsync<BilibiliResponse>(requestUrl, timeoutMs: 10_000, userAgent: "VocaDB/1.0 (admin@vocadb.net)");
			} catch (Exception x) when (x is HttpRequestException || x is WebException || x is JsonSerializationException || x is IOException) {
				log.Warn(x, "Unable to load Bilibili URL {0}", url);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(string.Format("Unable to load Bilibili URL: {0}", x.Message), x));
			}

			var authorId = response.Mid.ToString();
            int cid = response.Cid;

			if (string.IsNullOrEmpty(response.Title))
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "No title element");

			var title = HtmlEntity.DeEntitize(response.Title);
			var thumb = response.Pic ?? string.Empty;
			var author = response.Author ?? string.Empty;
			var created = response.CreatedAt;
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

	class BilibiliResponse {
		public string Author { get; set; }
		public int Cid { get; set; }
		[JsonProperty("created_at")]
		public DateTime? CreatedAt { get; set; }
		public int Mid { get; set; }
		public string Pic { get; set; }
		public string Title { get; set; }
	}

}
