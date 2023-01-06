#nullable disable

using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Helpers;

/// <summary>
/// Extension methods for generating URLs to entry images.
/// </summary>
public static class UrlHelperExtensionsForImages
{
	private static IAggregatedEntryImageUrlFactory GetImageUrlFactory(HttpContext context) => context.RequestServices.GetRequiredService<IAggregatedEntryImageUrlFactory>();

	private static VocaDbUrl GetUnknownImageUrl(IUrlHelper urlHelper)
	{
		return new VocaDbUrl(urlHelper.Content("~/Content/unknown.png"), UrlDomain.Main, System.UriKind.Relative);
	}

#nullable enable
	/// <summary>
	/// Returns an URL to entry thumbnail image, or placeholder if no image if specified.
	/// </summary>
	/// <param name="urlHelper">URL helper. Cannot be null.</param>
	/// <param name="imageInfo">Image information. Cannot be null.</param>
	/// <param name="size">Requested image size.</param>
	/// <returns>URL to the image thumbnail (may be placeholder).</returns>
	public static string ImageThumb(this IUrlHelper urlHelper, EntryThumbForApiContract? imageInfo, ImageSize size)
	{
		return imageInfo?.GetSmallestThumb(size).EmptyToNull() ?? GetUnknownImageUrl(urlHelper).Url;
	}
#nullable disable

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
	/// <param name="useUnknownImage">Use unknown image as fallback if image does not exist.</param>
	/// <returns>URL to the image thumbnail.</returns>
	public static string ImageThumb(this IUrlHelper urlHelper, IEntryImageInformation imageInfo, ImageSize size, bool fullUrl = false, bool useUnknownImage = true)
	{
		var unknown = useUnknownImage ? GetUnknownImageUrl(urlHelper) : VocaDbUrl.Empty;
		var url = GetImageUrlFactory(urlHelper.ActionContext.HttpContext).GetUrlWithFallback(imageInfo, size, unknown).ToAbsoluteIfNotMain();
		return fullUrl ? url.ToAbsolute().Url : url.Url;
	}
}