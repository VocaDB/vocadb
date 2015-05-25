using System.IO;
using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Helpers {

	/// <summary>
	/// Extension methods for generating URLs to entry images.
	/// </summary>
	public static class UrlHelperExtenderForImages {

		private static readonly IEntryThumbPersister imagePersister = new ServerEntryThumbPersister();

		private static ServerEntryImagePersisterOld EntryImagePersisterOld {
			get {
				return new ServerEntryImagePersisterOld();
			}
		}

		private static string GetUnknownImageUrl(UrlHelper urlHelper, IEntryImageInformation imageInfo) {
			return urlHelper.Content("~/Content/unknown.png");
		}

		private static bool ShouldExist(IEntryImageInformation imageInfo) {
			// Image should have MIME type, otherwise it's assumed not to exist.
			return !string.IsNullOrEmpty(imageInfo.Mime);
		}

		/// <summary>
		/// Generates an URL to <see cref="IEntryPictureFile" /> that are used as additional images for albums and artists.
		/// </summary>
		/// <param name="urlHelper">URL helper.</param>
		/// <param name="imageInfo">Image information. Can be null.</param>
		/// <param name="size">Desired image size.</param>
		/// <returns>Absolute URL to the image, or null if not found.</returns>
		public static string EntryPictureFile(this UrlHelper urlHelper, IEntryPictureFile imageInfo, ImageSize size) {
			
			return EntryImageOld(urlHelper, imageInfo, size);

		}

		/// <summary>
		/// Generates an URL to an entry image using the old folder structure for images.
		/// These are used for song lists and tags, and should eventually be migrated to the newer folder structure.
		/// </summary>
		/// <param name="urlHelper">URL helper.</param>
		/// <param name="imageInfo">Image information. Can be null.</param>
		/// <param name="size">Desired image size.</param>
		/// <param name="checkExists">
		/// Whether to check that the image actually exists on disk. 
		/// If this is true and the image doesn't exist, null will be returned.
		/// </param>
		/// <returns>Absolute URL to the image, or null if not found.</returns>
		public static string EntryImageOld(this UrlHelper urlHelper, IEntryImageInformation imageInfo, ImageSize size, bool checkExists = true) {
			
			if (imageInfo == null)
				return null;

			if (checkExists) {

				var path = EntryImagePersisterOld.GetPath(imageInfo, size);

				if (!File.Exists(path))
					return null;

			}

			return EntryImagePersisterOld.GetUrlAbsolute(imageInfo, size, WebHelper.IsSSL(HttpContext.Current.Request));

		}

		/// <summary>
		/// Returns an URL to entry thumbnail image.
		/// Currently only used for album and artist main images.
		/// 
		/// Gets the URL to the static images folder on disk if possible,
		/// otherwise gets the image from the DB.
		/// </summary>
		/// <param name="urlHelper">URL helper. Cannot be null.</param>
		/// <param name="imageInfo">Image information. Cannot be null.</param>
		/// <param name="size">Requested image size.</param>
		/// <param name="fullUrl">
		/// Whether the URL should always include the hostname and application path root.
		/// If this is false (default), the URL maybe either full (such as http://vocadb.net/Album/CoverPicture/123)
		/// or relative (such as /Album/CoverPicture/123).
		/// Usually this should be set to true if the image is to be referred from another domain.
		/// </param>
		/// <returns>URL to the image thumbnail.</returns>
		public static string ImageThumb(this UrlHelper urlHelper, IEntryImageInformation imageInfo, ImageSize size, bool fullUrl = false) {
			
			if (imageInfo == null)
				return null;

			var shouldExist = ShouldExist(imageInfo);
			string dynamicUrl = null;

			// Use MVC dynamic actions (instead of static file) when requesting original or an image that doesn't exist on disk.
			if (imageInfo.EntryType == EntryType.Album) {

				if (size == ImageSize.Original)
					dynamicUrl = urlHelper.Action("CoverPicture", "Album", new { id = imageInfo.Id, v = imageInfo.Version });
				else if (shouldExist && !imagePersister.HasImage(imageInfo, size))
					dynamicUrl = urlHelper.Action("CoverPictureThumb", "Album", new { id = imageInfo.Id, v = imageInfo.Version });

			} else if (imageInfo.EntryType == EntryType.Artist) {
				
				if (size == ImageSize.Original)
					dynamicUrl = urlHelper.Action("Picture", "Artist", new { id = imageInfo.Id, v = imageInfo.Version });
				else if (shouldExist && !imagePersister.HasImage(imageInfo, size))
					dynamicUrl = urlHelper.Action("PictureThumb", "Artist", new { id = imageInfo.Id, v = imageInfo.Version });

			}

			if (dynamicUrl != null) {				
				return (fullUrl ? AppConfig.HostAddress + dynamicUrl : dynamicUrl);
			}

			if (!shouldExist)
				return GetUnknownImageUrl(urlHelper, imageInfo);

			// For all other cases use the static file
			return imagePersister.GetUrlAbsolute(imageInfo, size, WebHelper.IsSSL(HttpContext.Current.Request));

		}

	}
}