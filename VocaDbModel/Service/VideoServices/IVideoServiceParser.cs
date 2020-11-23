using System;
using System.Threading.Tasks;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.VideoServices
{
	public interface IVideoServiceParser
	{
		Task<VideoTitleParseResult> GetTitleAsync(string id);
	}

	public class VideoTitleParseResult
	{
		public static VideoTitleParseResult Empty => new VideoTitleParseResult(true, null, null, null, null, null);

		public static VideoTitleParseResult CreateError(string error)
		{
			return new VideoTitleParseResult(false, error, null, null, null, null);
		}

		public static VideoTitleParseResult CreateSuccess(string title, string author, string authorId, string thumbUrl, int? length = null, string[] tags = null, DateTime? uploadDate = null, PVExtendedMetadata extendedMetadata = null)
		{
			return new VideoTitleParseResult(true, null, title, author, authorId, thumbUrl, length, tags, uploadDate, extendedMetadata);
		}

		public VideoTitleParseResult(bool success, string error, string title, string author, string authorId, string thumbUrl, int? length = null, string[] tags = null, DateTime? uploadDate = null, PVExtendedMetadata extendedMetadata = null)
		{
			Error = error;
			Success = success;
			Title = title ?? string.Empty;
			Author = author ?? string.Empty;
			AuthorId = authorId ?? string.Empty;
			ThumbUrl = thumbUrl ?? string.Empty;
			LengthSeconds = length;
			UploadDate = uploadDate;
			ExtendedMetadata = extendedMetadata;
			Tags = tags ?? new string[0];
		}

		/// <summary>
		/// Display name of the user who uploaded the video/song.
		/// This does not need to be unique.
		/// Optional field.
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// Identifier of the user who uploaded the video/song.
		/// For example on NND the users are identified by ID number
		/// instead of user name.
		/// This field can be used to uniquely identify the user.
		/// Optional field.
		/// </summary>
		public string AuthorId { get; set; }

		/// <summary>
		/// Error that occurred while parsing metadata.
		/// Null if there was no error.
		/// </summary>
		public string Error { get; set; }

		public PVExtendedMetadata ExtendedMetadata { get; set; }

		public int? LengthSeconds { get; set; }

		/// <summary>
		/// Whether the operation was successful.
		/// If false, the error should be specified in the <see cref="Error"/> field.
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		/// List of tags in the source system. Cannot be null.
		/// </summary>
		/// <remarks>
		/// These are tag names for example on NND.
		/// </remarks>
		public string[] Tags { get; set; }

		/// <summary>
		/// Parsed title.
		/// Required field, cannot be null if the operation succeeded.
		/// </summary>
		public string Title { get; set; }

		public string ThumbUrl { get; set; }

		public DateTime? UploadDate { get; set; }
	}
}
