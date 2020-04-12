namespace VocaDb.Model.Domain.Images {

	public interface IEntryImageUrlFactory {

		/// <summary>
		/// Gets an absolute URL for internal and external access to an image.
		/// </summary>
		/// <param name="picture">Image information. Cannot be null.</param>
		/// <param name="size">Image size.</param>
		/// <returns>
		/// Absolute URL to the image, for example "https://static.vocadb.net/img/Album/Orig/Full/123.jpg".
		/// Cannot be null, but not guaranteed to exist (use <see cref="HasImage"/> to make sure file exists).
		/// </returns>
		VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size);

		/// <summary>
		/// Checks whether a specific image exists in the store.
		/// </summary>
		/// <param name="picture">Image information. Cannot be null.</param>
		/// <param name="size">Image size.</param>
		/// <returns>True if the image exists, otherwise false.</returns>
		bool HasImage(IEntryImageInformation picture, ImageSize size);

		bool IsSupported(IEntryImageInformation picture, ImageSize size);

	}

	/// <summary>
	/// Extensions to <see cref="IEntryImageUrlFactory"/>.
	/// </summary>
	public static class IEntryImageUrlFactoryExtender {

		public static VocaDbUrl GetUrlWithFallback(this IEntryImageUrlFactory urlFactory, IEntryImageInformation imageInfo, ImageSize size, VocaDbUrl fallbackUrl) {

			if (imageInfo == null || !imageInfo.ShouldExist() || !urlFactory.HasImage(imageInfo, size))
				return fallbackUrl;

			return urlFactory.GetUrl(imageInfo, size);

		}

		/// <summary>
		/// Gets URL to image, optionally verifying that it exists.
		/// </summary>
		/// <param name="persister">Persister. Cannot be null.</param>
		/// <param name="picture">Image information. Cannot be null.</param>
		/// <param name="size">Image size.</param>
		/// <param name="checkExists">If true, verify that the image exists. If false, no verification is done.</param>
		/// <returns>
		/// URL to image, if image exists or <paramref name="checkExists"/> is false. 
		/// If <paramref name="checkExists"/> is true and image does not exist, this will be <see cref="VocaDbUrl.Empty"/>.
		/// </returns>
		public static VocaDbUrl GetUrl(this IEntryImageUrlFactory persister, IEntryImageInformation picture, ImageSize size, bool checkExists) {

			if (checkExists && !persister.HasImage(picture, size))
				return VocaDbUrl.Empty;

			return persister.GetUrl(picture, size);

		}

		public static string GetUrlAbsolute(this IEntryImageUrlFactory persister, IEntryImageInformation picture, ImageSize size, bool checkExists = false) {
			return persister.GetUrl(picture, size, checkExists).Url;
		}

	}

}
