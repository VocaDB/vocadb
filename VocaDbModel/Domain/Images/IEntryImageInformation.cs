
namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Provides information about an entry image (both original full size images and thumbnails).
	/// </summary>
	public interface IEntryImageInformation {

		/// <summary>
		/// Type of entry.
		/// Determines the folder in which the image file is saved (and thus the URL).
		/// </summary>
		EntryType EntryType { get; }

		/// <summary>
		/// Image identifier. 
		/// This may be the entry Id, for example for album/artist main images and for entries with only one image (songlists and tags).
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Image MIME type.
		/// Used to determine file extension.
		/// 
		/// If this is null or empty, it is assumed that there is no image. 
		/// Images without MIME type (or unknown MIME type) are not supported.
		/// </summary>
		string Mime { get; }

		/// <summary>
		/// Image/entry version.
		/// This is appended to the URL for caching.
		/// If the version changes, the image is assumed to be possibly changed as well.
		/// If the image does not support versioning this will always be 0.
		/// </summary>
		int Version { get; }

	}

	public static class EntryImageInformationExtensions {

		/// <summary>
		/// Tests whether image file should exist.
		/// Image file is assumed to exist if it has MIME type.
		/// However, it is still not guaranteed, if the file is removed from disk.
		/// </summary>
		/// <param name="image">Image information.</param>
		/// <returns>True if image is assumed to exist. Otherwise false.</returns>
		public static bool ShouldExist(this IEntryImageInformation image) {
			return image != null && !string.IsNullOrEmpty(image.Mime);
		}

	}

}
