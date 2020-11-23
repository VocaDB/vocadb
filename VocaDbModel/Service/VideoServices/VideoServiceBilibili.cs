using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.VideoServices
{

	public class VideoServiceBilibili : VideoService
	{

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public static readonly RegexLinkMatcher[] Matchers =
			{
				new RegexLinkMatcher("acg.tv/av{0}", @"www.bilibili.com/video/av(\d+)"),
				new RegexLinkMatcher("acg.tv/av{0}", @"www.bilibili.com/video/(BV\w+)"),
				new RegexLinkMatcher("acg.tv/av{0}", @"acg.tv/av(\d+)"),
				new RegexLinkMatcher("acg.tv/av{0}", @"www.bilibili.tv/video/av(\d+)"),
				new RegexLinkMatcher("acg.tv/av{0}", @"bilibili.kankanews.com/video/av(\d+)")
			};

		public VideoServiceBilibili()
			: base(PVService.Bilibili, null, Matchers) { }

		public override async Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle)
		{

			var id = GetIdByUrl(url);

			if (string.IsNullOrEmpty(id))
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.NoMatcher, "No matcher");

			var requestUrl = "https://api.bilibili.com/x/web-interface/view?" + (id.StartsWith("BV") ? $"bvid={id}" : $"aid={id}");

			BilibiliResponse response;

			try
			{
				response = await JsonRequest.ReadObjectAsync<BilibiliResponse>(requestUrl, timeout: TimeSpan.FromSeconds(10), userAgent: "VocaDB/1.0 (admin@vocadb.net)");
			}
			catch (Exception x) when (x is HttpRequestException || x is WebException || x is JsonSerializationException || x is IOException)
			{
				log.Warn(x, "Unable to load Bilibili URL {0}", url);
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(string.Format("Unable to load Bilibili URL: {0}", x.Message), x));
			}

			var authorId = response.Data.Owner.Mid.ToString();
			var aid = response.Data.Aid;
			var bvid = response.Data.Bvid;
			var cid = response.Data.Cid;

			if (!getTitle)
			{
				return VideoUrlParseResult.CreateOk(url, PVService.Bilibili, aid.ToString(), VideoTitleParseResult.Empty);
			}

			if (string.IsNullOrEmpty(response.Data.Title))
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "No title element");

			var title = HtmlEntity.DeEntitize(response.Data.Title);
			var thumb = response.Data.Pic ?? string.Empty;
			var author = response.Data.Owner.Name ?? string.Empty;
			var created = response.Data.PubDate;
			var length = response.Data.Duration;

			var metadata = new PVExtendedMetadata(new BiliMetadata
			{
				Aid = aid,
				Bvid = bvid,
				Cid = cid
			});

			return VideoUrlParseResult.CreateOk(url, PVService.Bilibili, aid.ToString(),
				VideoTitleParseResult.CreateSuccess(title, author, authorId, thumb, length: length, uploadDate: created, extendedMetadata: metadata));

		}

		public override IEnumerable<string> GetUserProfileUrls(string authorId)
		{
			return new[] {
				string.Format("https://space.bilibili.com/{0}", authorId),
				string.Format("http://space.bilibili.com/{0}", authorId),
				string.Format("http://space.bilibili.com/{0}/#!/index", authorId)
			};
		}

		public override string GetUrlById(string id, PVExtendedMetadata _) => $"https://www.bilibili.com/video/av{id}";

	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class BiliMetadata
	{
		[DataMember]
		public int Aid { get; set; }
		[DataMember]
		public string Bvid { get; set; }
		[DataMember]
		public int Cid { get; set; }
	}

	class BilibiliResponse
	{
		public BilibiliResponseData Data { get; set; }
	}

	class BilibiliResponseData
	{
		public int Aid { get; set; }
		public string Bvid { get; set; }
		public int Cid { get; set; }
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime? PubDate { get; set; }
		public int Duration { get; set; }
		public BilibiliResponseDataOwner Owner { get; set; }
		public string Pic { get; set; }
		public string Title { get; set; }
	}

	class BilibiliResponseDataOwner
	{
		public int Mid { get; set; }
		public string Name { get; set; }
	}

}
