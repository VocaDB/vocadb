
namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Provides information about an entry image.
	/// 
	/// This interface is used for saving entry images to disk.
	/// Currently this means thumbnails for songlists and tags, but
	/// will be expanded to album/artist thumbnails soon.
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
		/// Determines file extension.
		/// Unknown MIME type (null or empty) is not supported.
		/// </summary>
		string Mime { get; }

		/// <summary>
		/// Image/entry version.
		/// This is appended to the URL for caching.
		/// If the version changes, the image is assumed to be possibly changed as well.
		/// </summary>
		int Version { get; }

	}

}
