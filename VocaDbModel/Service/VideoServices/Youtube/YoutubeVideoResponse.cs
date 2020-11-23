namespace VocaDb.Model.Service.VideoServices.Youtube
{

	public class YoutubeVideoResponse : YoutubeResponse<YoutubeVideoItem> { }

	public class YoutubeVideoItem : YoutubeItem<YoutubeVideoSnippet>
	{

		public YoutubeVideoContentDetails ContentDetails { get; set; }

	}

	public class YoutubeVideoSnippet : Snippet
	{

		public string ChannelId { get; set; }

		public string ChannelTitle { get; set; }

		public YoutubeVideoThumbnails Thumbnails { get; set; }

	}

	public class YoutubeVideoThumbnails
	{

		public YoutubeVideoThumbnail Default { get; set; }

	}

	public class YoutubeVideoThumbnail
	{

		public string Url { get; set; }

	}

	public class YoutubeVideoContentDetails
	{

		public string Duration { get; set; }

	}

}
