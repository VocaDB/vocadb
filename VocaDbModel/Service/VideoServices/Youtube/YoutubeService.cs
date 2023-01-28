#nullable disable

using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.VideoServices.Youtube;

public class YoutubeService
{
	private const string VideoQueryFormat =
		"https://www.googleapis.com/youtube/v3/videos?part=snippet,contentDetails&key={0}&id={1}";

	private readonly string _apiKey;

	private string Url(string id) => string.Format(VideoQueryFormat, _apiKey, id);

	public YoutubeService(string apiKey)
	{
		_apiKey = apiKey;
	}

	public Task<YoutubeVideoResponse> VideoAsync(string id) => JsonRequest.ReadObjectAsync<YoutubeVideoResponse>(Url(id));
}
