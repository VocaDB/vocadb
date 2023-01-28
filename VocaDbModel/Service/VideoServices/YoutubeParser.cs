#nullable disable

using System.Xml;
using VocaDb.Model.Service.VideoServices.Youtube;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices;

public class YoutubeParser : IVideoServiceParser
{
	private YoutubeService _service;

	public YoutubeParser()
	{
		_service = new YoutubeService(AppConfig.YoutubeApiKey);
	}

	private int? GetLength(YoutubeVideoItem video)
	{
		if (video.ContentDetails == null || string.IsNullOrEmpty(video.ContentDetails.Duration))
			return null;

		TimeSpan timespan;

		try
		{
			timespan = XmlConvert.ToTimeSpan(video.ContentDetails.Duration);
		}
		catch (FormatException)
		{
			return null;
		}

		return (int?)timespan.TotalSeconds;
	}

	private DateTime? GetPublishDate(YoutubeVideoItem video)
	{
		return (video.Snippet.PublishedAt.HasValue ? (DateTime?)video.Snippet.PublishedAt.Value.Date : null);
	}

	private VideoTitleParseResult GetTitle(YoutubeVideoResponse result)
	{
		if (!result.Items.Any())
		{
			return VideoTitleParseResult.Empty;
		}

		var video = result.Items.First();
		var thumbUrl = video.Snippet.Thumbnails.Default != null ? video.Snippet.Thumbnails.Default.Url : string.Empty;
		var length = GetLength(video);
		var author = video.Snippet.ChannelTitle;
		var authorId = video.Snippet.ChannelId;
		var publishDate = GetPublishDate(video);

		return VideoTitleParseResult.CreateSuccess(video.Snippet.Title, author, authorId, thumbUrl, length, uploadDate: publishDate);
	}

	public async Task<VideoTitleParseResult> GetTitleAsync(string id)
	{
		YoutubeVideoResponse result;
		try
		{
			result = await _service.VideoAsync(id);
		}
		catch (HttpRequestException x)
		{
			return VideoTitleParseResult.CreateError(x.Message);
		}

		return GetTitle(result);
	}
}
