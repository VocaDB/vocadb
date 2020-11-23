using System;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices
{

	public class VideoUrlParseResult
	{

		public static VideoParseException GetException(VideoUrlParseResultType resultType, string url)
		{

			switch (resultType)
			{
				case VideoUrlParseResultType.NoMatcher:
					return new VideoParseException(string.Format("No matcher defined (url {0})", url));

				case VideoUrlParseResultType.LoadError:
					return new VideoParseException(string.Format("Unable to load metadata (url {0})", url));

				case VideoUrlParseResultType.Duplicate:
					return new VideoParseException(string.Format("Duplicate entry (url {0})", url));

				case VideoUrlParseResultType.OtherError:
					return new VideoParseException(string.Format("Unable to process PV link (url {0})", url));

				default:
					return null;
			}

		}

		private VideoUrlParseResult(VideoUrlParseResultType resultType, string url, VideoParseException exception)
		{

			ResultType = resultType;
			Url = url;
			Exception = exception ?? GetException(resultType, url);

		}

		private VideoUrlParseResult(string url, PVService service, string id, VideoTitleParseResult meta)
		{

			Url = url;
			Service = service;
			Id = id;
			Title = meta.Title ?? string.Empty;
			Author = meta.Author ?? string.Empty;
			AuthorId = meta.AuthorId ?? string.Empty;
			ExtendedMetadata = meta.ExtendedMetadata;
			ThumbUrl = meta.ThumbUrl ?? string.Empty;
			LengthSeconds = meta.LengthSeconds;
			Tags = meta.Tags;
			UploadDate = meta.UploadDate;

			ResultType = VideoUrlParseResultType.Ok;

		}

		public static VideoUrlParseResult CreateError(string url, VideoUrlParseResultType resultType, VideoParseException exception = null)
		{

			return new VideoUrlParseResult(resultType, url, exception);

		}

		public static VideoUrlParseResult CreateError(string url, VideoUrlParseResultType resultType, string message)
		{

			return CreateError(url, resultType, new VideoParseException(message));

		}

		public static VideoUrlParseResult CreateOk(string url, PVService service, string id, VideoTitleParseResult meta)
		{

			return new VideoUrlParseResult(url, service, id, meta);

		}

		public string Author { get; set; }

		public string AuthorId { get; set; }

		/// <summary>
		/// Exception. Cannot be null if result type is anything but Ok.
		/// </summary>
		public VideoParseException Exception { get; set; }

		public PVExtendedMetadata ExtendedMetadata { get; set; }

		public string Id { get; set; }

		public bool IsOk => ResultType == VideoUrlParseResultType.Ok;

		public int? LengthSeconds { get; set; }

		public VideoUrlParseResultType ResultType { get; set; }

		public PVService Service { get; set; }

		public string[] Tags { get; set; }

		public string Title { get; set; }

		public string ThumbUrl { get; set; }

		public string Url { get; set; }

		public DateTime? UploadDate { get; set; }

	}

	public enum VideoUrlParseResultType
	{

		Ok,

		/// <summary>
		/// No matcher could be identified for the URL
		/// </summary>
		NoMatcher,

		/// <summary>
		/// Loading of title or other metadata failed
		/// </summary>
		LoadError,

		/// <summary>
		/// This PV has already been added to DB
		/// </summary>
		Duplicate,

		/// <summary>
		/// Other error
		/// </summary>
		OtherError

	}

}
