// Comments from: https://github.com/blackjack4494/yt-dlc/blob/1b3f7c9a7ef0f107819b59479811c5f0066f2def/youtube_dlc/extractor/common.py

namespace VocaDb.BandcampMetadataExtractor
{
	public record BandcampInfo
	{
		/// <summary>
		/// Track/video description.
		/// </summary>
		public string? Description { get; init; }

		/// <summary>
		/// Length of the video in seconds, as an integer or float.
		/// </summary>
		public double? Duration { get; init; }

		/// <summary>
		/// Video identifier.
		/// </summary>
		public string? Id { get; init; }

		/// <summary>
		/// Full URL to a video thumbnail image.
		/// </summary>
		public string? Thumbnail { get; init; }

		/// <summary>
		/// Video title, unescaped.
		/// </summary>
		public string? Title { get; init; }

		/// <summary>
		/// Video upload date (YYYYMMDD).
		/// If not explicitly set, calculated from timestamp.
		/// </summary>
		public string? UploadDate { get; init; }

		/// <summary>
		/// Full name of the video uploader.
		/// </summary>
		public string? Uploader { get; init; }

		/// <summary>
		/// Nickname or id of the video uploader.
		/// </summary>
		public string? UploaderId { get; init; }

		/// <summary>
		/// The URL to the video webpage, if given to youtube-dlc it
		/// should allow to get the same result again. (It will be set
		/// by YoutubeDL if it's missing)
		/// </summary>
		public string? WebpageUrl { get; init; }
	}
}
