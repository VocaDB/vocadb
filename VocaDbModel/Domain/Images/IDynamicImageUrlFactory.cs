namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Returns "dynamic" MVC controller URL to database-stored images,
	/// that is artist and album original images.
	/// </summary>
	public interface IDynamicImageUrlFactory {

		/// <summary>
		/// Gets relative URL to image.
		/// </summary>
		/// <returns>Relative URL, for example "Album/39/CoverPicture". Can be null, if image is not accessible this way.</returns>
		string GetRelativeDynamicUrl(IEntryImageInformation image, ImageSize size);

	}

}
