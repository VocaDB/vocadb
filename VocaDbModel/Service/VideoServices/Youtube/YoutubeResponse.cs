namespace VocaDb.Model.Service.VideoServices.Youtube {

	public class YoutubeResponse<T> where T : IYoutubeItem {

		public T[] Items { get; set; }

		public string NextPageToken { get; set; }

		public YoutubePageInfo PageInfo { get; set; }

	}

}
