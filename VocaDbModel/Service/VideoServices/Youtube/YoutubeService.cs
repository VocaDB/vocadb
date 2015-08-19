using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.VideoServices.Youtube {

	public class YoutubeService {

		private const string videoQueryFormat =
			"https://www.googleapis.com/youtube/v3/videos?part=snippet,contentDetails&key={0}&id={1}";

		private readonly string apiKey;

		public YoutubeService(string apiKey) {
			this.apiKey = apiKey;
		}

		public YoutubeVideoResponse Video(string id) {

			var url = string.Format(videoQueryFormat, apiKey, id);

			return JsonRequest.ReadObject<YoutubeVideoResponse>(url);

		}

	}
}
