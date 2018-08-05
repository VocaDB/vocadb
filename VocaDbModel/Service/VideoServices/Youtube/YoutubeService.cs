using System.Threading.Tasks;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.VideoServices.Youtube {

	public class YoutubeService {

		private const string videoQueryFormat =
			"https://www.googleapis.com/youtube/v3/videos?part=snippet,contentDetails&key={0}&id={1}";

		private readonly string apiKey;

		private string Url(string id) => string.Format(videoQueryFormat, apiKey, id);

		public YoutubeService(string apiKey) {
			this.apiKey = apiKey;
		}

		public YoutubeVideoResponse Video(string id) => JsonRequest.ReadObject<YoutubeVideoResponse>(Url(id));
		public Task<YoutubeVideoResponse> VideoAsync(string id) => JsonRequest.ReadObjectAsync<YoutubeVideoResponse>(Url(id));

	}
}
