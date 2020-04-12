using System.Web.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Helpers {

	/// <summary>
	/// Extension methods for generating URLs to entry images.
	/// </summary>
	public static class UrlHelperExtenderForImages {

		private static IAggregatedEntryImageUrlFactory ImageUrlFactory => DependencyResolver.Current.GetService<IAggregatedEntryImageUrlFactory>();

		private static VocaDbUrl GetUnknownImageUrl(UrlHelper urlHelper) {
			return new VocaDbUrl(urlHelper.Content("~/Content/unknown.png"), UrlDomain.Main, System.UriKind.Relative);
		}

		/// <summary>
		/// Returns an URL to entry thumbnail image, or placeholder if no image if specified.
		/// </summary>
		/// <param name="urlHelper">URL helper. Cannot be null.</param>
		/// <param name="imageInfo">Image information. Cannot be null.</param>
		/// <param name="size">Requested image size.</param>
		/// <returns>URL to the image thumbnail (may be placeholder).</returns>
		public static string ImageThumb(this UrlHelper urlHelper, EntryThumbForApiContract imageInfo, ImageSize size) {

			return imageInfo?.GetSmallestThumb(size).EmptyToNull() ?? GetUnknownImageUrl(urlHelper).Url;

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
			
			var unknown = GetUnknownImageUrl(urlHelper);
			var url = ImageUrlFactory.GetUrlWithFallback(imageInfo, size, unknown);
			return fullUrl || url.Domain == UrlDomain.Static ? url.ToAbsolute().Url : url.Url;

		}

	}
}