using System.Runtime.Serialization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts
{

	/// <summary>
	/// Entry thumbnail for API.
	/// Contains URLs to thumbnails of different sizes.
	/// </summary>
	/// <remarks>
	/// Default sizes are described in <see cref="ImageSize"/>, but the sizes might vary depending on entry type.
	/// For example, song thumbnails have different sizes.
	/// </remarks>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryThumbForApiContract
	{

		public static EntryThumbForApiContract Create(IEntryImageInformation image, IAggregatedEntryImageUrlFactory thumbPersister,
			ImageSizes sizes = ImageSizes.All)
		{

			if (thumbPersister == null || string.IsNullOrEmpty(image?.Mime))
				return null;

			return new EntryThumbForApiContract(image, thumbPersister, sizes);

		}

		public EntryThumbForApiContract() { }

		/// <summary>
		/// Initializes image data.
		/// </summary>
		/// <param name="image">Image information. Cannot be null.</param>
		/// <param name="thumbPersister">Thumb persister. Cannot be null.</param>
		/// <param name="sizes">Sizes to generate. If Nothing, no image URLs will be generated.</param>
		public EntryThumbForApiContract(IEntryImageInformation image, IAggregatedEntryImageUrlFactory thumbPersister,
			ImageSizes sizes = ImageSizes.All)
		{

			ParamIs.NotNull(() => image);
			ParamIs.NotNull(() => thumbPersister);

			Mime = image.Mime;

			if (string.IsNullOrEmpty(image.Mime) && sizes != ImageSizes.Nothing)
				return;

			if (sizes.HasFlag(ImageSizes.Original))
				UrlOriginal = thumbPersister.GetUrlAbsolute(image, ImageSize.Original);

			if (sizes.HasFlag(ImageSizes.SmallThumb))
				UrlSmallThumb = thumbPersister.GetUrlAbsolute(image, ImageSize.SmallThumb);

			if (sizes.HasFlag(ImageSizes.Thumb))
				UrlThumb = thumbPersister.GetUrlAbsolute(image, ImageSize.Thumb);

			if (sizes.HasFlag(ImageSizes.TinyThumb))
				UrlTinyThumb = thumbPersister.GetUrlAbsolute(image, ImageSize.TinyThumb);

		}

		/// <summary>
		/// MIME type, for example "image/jpeg".
		/// </summary>
		[DataMember]
		public string Mime { get; set; }

		/// <summary>
		/// URL to original image.
		/// </summary>
		[DataMember]
		public string UrlOriginal { get; set; }

		/// <summary>
		/// URL to small thumbnail.
		/// Default size is 150x150px.
		/// </summary>
		[DataMember]
		public string UrlSmallThumb { get; set; }

		/// <summary>
		/// URL to large thumbnail.
		/// Default size is 250x250px.
		/// </summary>
		[DataMember]
		public string UrlThumb { get; set; }

		/// <summary>
		/// URL to tiny thumbnail.
		/// Default size is 70x70px.
		/// </summary>
		[DataMember]
		public string UrlTinyThumb { get; set; }

		/// <summary>
		/// Gets the smallest available thumbnail URL that is preferably larger than the specified size.
		/// </summary>
		/// <param name="preferLargerThan">
		/// Prefer image sizes equal or larger than this. If nothing else is available, smaller size is allowed as well.
		/// </param>
		/// <returns>Thumbnail URL. Can be null or empty.</returns>
		public string GetSmallestThumb(ImageSize preferLargerThan)
		{

			switch (preferLargerThan)
			{
				case ImageSize.TinyThumb:
					return UrlTinyThumb ?? UrlSmallThumb ?? UrlThumb ?? UrlOriginal;
				case ImageSize.SmallThumb:
					return UrlSmallThumb ?? UrlThumb ?? UrlTinyThumb ?? UrlOriginal;
				case ImageSize.Thumb:
					return UrlThumb ?? UrlSmallThumb ?? UrlTinyThumb ?? UrlOriginal;
				default:
					return UrlOriginal ?? UrlThumb ?? UrlSmallThumb ?? UrlTinyThumb;
			}

		}

	}

}
