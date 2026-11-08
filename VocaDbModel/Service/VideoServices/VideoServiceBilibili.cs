using System.Globalization;
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
	private const long BvAidLimit = 1L << 51;
	private const string BvTable = "FcwAPNKTMug3GV5Lj7EJnHpWsx4tb8haYeviqBz6rkCy12mUSDQX9RdoZf";
	private const long BvXor = 23442827791579;
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

		if (!getTitle)
		{
			string? aidId = null;

			if (id.StartsWith("BV", StringComparison.Ordinal))
			{
				aidId = GetAidByBvId(id);
			}
			else if (long.TryParse(id, NumberStyles.None, CultureInfo.InvariantCulture, out var aidNumber)
				&& aidNumber > 0 && aidNumber < BvAidLimit)
			{
				aidId = aidNumber.ToString(CultureInfo.InvariantCulture);
			}

			if (aidId == null)
				return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.OtherError, "Invalid Bilibili ID");

			return VideoUrlParseResult.CreateOk(url, PVService.Bilibili, aidId, VideoTitleParseResult.Empty);
		}

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

		if (string.IsNullOrEmpty(response.Data.Title))
			return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "No title element");

		var title = HtmlEntity.DeEntitize(response.Data.Title);
		var thumb = response.Data.Pic ?? string.Empty;
		var author = response.Data.Owner.Name ?? string.Empty;
		var created = response.Data.PubDate;
		var length = response.Data.Duration;
		var description = response.Data.Desc;

		var metadata = new PVExtendedMetadata(new BiliMetadata
		{
			Aid = aid,
			Bvid = bvid,
			Cid = cid
		});

		return VideoUrlParseResult.CreateOk(url, PVService.Bilibili, aid.ToString(),
			VideoTitleParseResult.CreateSuccess(title, author, authorId, thumb, length: length, uploadDate: created, extendedMetadata: metadata, description: description));
	}

	private static string? GetAidByBvId(string bvId)
	{
		if (bvId.Length != 12 || !bvId.StartsWith("BV1", StringComparison.Ordinal))
			return null;

		var characters = bvId.ToCharArray();
		(characters[3], characters[9]) = (characters[9], characters[3]);
		(characters[4], characters[7]) = (characters[7], characters[4]);
		var sum = 0L;

		for (var i = 3; i < characters.Length; i++)
		{
			var index = BvTable.IndexOf(characters[i]);

			if (index < 0)
				return null;

			sum = sum * 58 + index;
		}

		if (sum < BvAidLimit || sum >= BvAidLimit * 2)
			return null;

		var aid = (sum & (BvAidLimit - 1)) ^ BvXor;
		return aid > 0 ? aid.ToString(CultureInfo.InvariantCulture) : null;
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
	public long Aid { get; init; }
	[DataMember]
	public string Bvid { get; init; } = default!;
	[DataMember]
	public long Cid { get; init; }
}

class BilibiliResponse
{
	public long Code { get; init; } = default!;
	public BilibiliResponseData? Data { get; init; } = default;
}

class BilibiliResponseData
{
	public long Aid { get; init; }
	public string Bvid { get; init; } = default!;
	public long Cid { get; init; }
	public string Desc { get; init; } = default!;
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
