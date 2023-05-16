using System.Net;
using System.Runtime.Serialization;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.VideoServices;

public class VideoServiceBilibili : VideoService
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

	public static readonly RegexLinkMatcher[] Matchers =
		{
			new RegexLinkMatcher("acg.tv/av{0}", @"bilibili.com/video/av(\d+)"),
			new RegexLinkMatcher("acg.tv/av{0}", @"bilibili.com/video/(BV\w+)"),
			new RegexLinkMatcher("acg.tv/av{0}", @"acg.tv/av(\d+)"),
			new RegexLinkMatcher("acg.tv/av{0}", @"bilibili.tv/video/av(\d+)"),
			new RegexLinkMatcher("acg.tv/av{0}", @"bilibili.kankanews.com/video/av(\d+)"),
			new RegexLinkMatcher("acg.tv/av{0}", @"b23.tv/av(\d+)"),
			new RegexLinkMatcher("acg.tv/av{0}", @"b23.tv/(BV\w+)")
		};

	public VideoServiceBilibili()
		: base(PVService.Bilibili, null, Matchers) { }

	public override async Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle)
	{
		var id = GetIdByUrl(url);

		if (string.IsNullOrEmpty(id))
			return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.NoMatcher, "No matcher");

		var requestUrl = "https://api.bilibili.com/x/web-interface/view?" + (id.StartsWith("BV") ? $"bvid={id}" : $"aid={id}");

		BilibiliResponse? response;

		try
		{
			s_log.Info($"Loading Bilibili URL {url.Replace(Environment.NewLine, "")}");
			response = await JsonRequest.ReadObjectAsync<BilibiliResponse>(requestUrl, timeout: TimeSpan.FromSeconds(10), userAgent: "VocaDB/1.0 (admin@vocadb.net)");
		}
		catch (Exception x) when (x is HttpRequestException or WebException or JsonSerializationException or IOException)
		{
			s_log.Warn(x, $"Unable to load Bilibili URL {url.Replace(Environment.NewLine, "")}");
			return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException($"Unable to load Bilibili URL: {x.Message}", x));
		}

		// If the video cannot be viewed,
		// web returns the error message "啊叻？视频不见了？" (あれ? the video's gone?)
		// API simultaneously returns the message "稿件不可见" (upload cannot be viewed)
		if (response == null || response.Data == null || response.Code == 62002)
		{
			return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Video cannot be viewed");
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
			$"https://space.bilibili.com/{authorId}",
			$"http://space.bilibili.com/{authorId}",
			$"http://space.bilibili.com/{authorId}/#!/index"
		};
	}

	public override string GetUrlById(string id, PVExtendedMetadata _) => $"https://www.bilibili.com/video/av{id}";
}

[DataContract(Namespace = Schemas.VocaDb)]
public class BiliMetadata
{
	[DataMember]
	public int Aid { get; init; }
	[DataMember]
	public string Bvid { get; init; } = default!;
	[DataMember]
	public int Cid { get; init; }
}

class BilibiliResponse
{
	public int Code { get; init; } = default!;
	public BilibiliResponseData? Data { get; init; } = default;
}

class BilibiliResponseData
{
	public int Aid { get; init; }
	public string Bvid { get; init; } = default!;
	public int Cid { get; init; }
	[JsonConverter(typeof(UnixDateTimeConverter))]
	public DateTime? PubDate { get; init; }
	public int Duration { get; init; }
	public BilibiliResponseDataOwner Owner { get; init; } = default!;
	public string Pic { get; init; } = default!;
	public string Title { get; init; } = default!;
}

class BilibiliResponseDataOwner
{
	public long Mid { get; init; }
	public string Name { get; init; } = default!;
}
