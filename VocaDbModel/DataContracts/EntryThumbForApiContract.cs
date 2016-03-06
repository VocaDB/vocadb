using System.Runtime.Serialization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts {

	/// <summary>
	/// Entry thumbnail for API.
	/// Contains URLs to thumbnails of different sizes.
	/// Does not include URL to original picture at the moment because that is loaded differently.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryThumbForApiContract {

		public EntryThumbForApiContract() { }

		/// <summary>
		/// Initializes image data.
		/// </summary>
		/// <param name="image">Image information. Cannot be null.</param>
		/// <param name="thumbPersister">Thumb persister. Cannot be null.</param>
		/// <param name="ssl">Whether to generate SSL URLs.</param>
		/// <param name="sizes">Sizes to generate. If Nothing, no image URLs will be generated.</param>
		public EntryThumbForApiContract(IEntryImageInformation image, IEntryImagePersister thumbPersister, bool ssl,
			ImageSizes sizes = ImageSizes.All) {

			ParamIs.NotNull(() => image);
			ParamIs.NotNull(() => thumbPersister);

			if (string.IsNullOrEmpty(image.Mime) && sizes != ImageSizes.Nothing)
				return;

			if (sizes.HasFlag(ImageSizes.SmallThumb))
				UrlSmallThumb = thumbPersister.GetUrlAbsolute(image, ImageSize.SmallThumb, ssl);

			if (sizes.HasFlag(ImageSizes.Thumb))
				UrlThumb = thumbPersister.GetUrlAbsolute(image, ImageSize.Thumb, ssl);

			if (sizes.HasFlag(ImageSizes.TinyThumb))
				UrlTinyThumb = thumbPersister.GetUrlAbsolute(image, ImageSize.TinyThumb, ssl);				

		}

		[DataMember]
		public string UrlSmallThumb { get; set; }

		[DataMember]
		public string UrlThumb { get; set; }

		[DataMember]
		public string UrlTinyThumb { get; set; }

		/// <summary>
		/// Gets the smallest available thumbnail URL that is preferably larger than the specified size.
		/// </summary>
		/// <param name="preferLargerThan">Prefer image sizes equal or larger than this. If nothing else is available, smaller size is allowed as well.</param>
		/// <returns>Thumbnail URL. Can be null or empty.</returns>
		public string GetSmallestThumb(ImageSize preferLargerThan) {
			
			switch (preferLargerThan) {
				case ImageSize.TinyThumb:
					return UrlTinyThumb ?? UrlSmallThumb ?? UrlThumb;
				case ImageSize.SmallThumb:
					return UrlSmallThumb ?? UrlThumb ?? UrlTinyThumb;
				default:
					return UrlThumb ?? UrlSmallThumb ?? UrlTinyThumb;
			}				

		}

	}

}
