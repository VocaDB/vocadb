#nullable disable

using System.Diagnostics.CodeAnalysis;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices
{
	public class VideoUrlParseResult
	{
#nullable enable
		public static VideoParseException? GetException(VideoUrlParseResultType resultType, string url) => resultType switch
		{
			VideoUrlParseResultType.NoMatcher => new VideoParseException($"No matcher defined (url {url})"),
			VideoUrlParseResultType.LoadError => new VideoParseException($"Unable to load metadata (url {url})"),
			VideoUrlParseResultType.Duplicate => new VideoParseException($"Duplicate entry (url {url})"),
			VideoUrlParseResultType.OtherError => new VideoParseException($"Unable to process PV link (url {url})"),
			_ => null,
		};
#nullable disable

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

#nullable enable
		public static VideoUrlParseResult CreateError(string url, VideoUrlParseResultType resultType, VideoParseException? exception = null)
		{
			return new VideoUrlParseResult(resultType, url, exception);
		}

		public static VideoUrlParseResult CreateError(string url, VideoUrlParseResultType resultType, string? message)
		{
			return CreateError(url, resultType, new VideoParseException(message));
		}

		public static VideoUrlParseResult CreateOk(string url, PVService service, string id, VideoTitleParseResult meta)
		{
			return new VideoUrlParseResult(url, service, id, meta);
		}
#nullable disable

		public string Author { get; set; }

		public string AuthorId { get; set; }

#nullable enable
		/// <summary>
		/// Exception. Cannot be null if result type is anything but Ok.
		/// </summary>
		public VideoParseException? Exception { get; set; }

		public PVExtendedMetadata? ExtendedMetadata { get; set; }
#nullable disable

		public string Id { get; set; }

#nullable enable
		[MemberNotNullWhen(false, nameof(Exception))]
		public bool IsOk => ResultType == VideoUrlParseResultType.Ok;

		public int? LengthSeconds { get; set; }

		public VideoUrlParseResultType ResultType { get; set; }

		public PVService Service { get; set; }
#nullable disable

		public string[] Tags { get; set; }

		public string Title { get; set; }

		public string ThumbUrl { get; set; }

#nullable enable
		public string Url { get; set; }

		public DateTime? UploadDate { get; set; }
#nullable disable
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
