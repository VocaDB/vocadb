using System.Drawing;
using System.IO;

namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Writes entry images to a store (such as disk) and loads them back as needed.
	/// </summary>
	public interface IEntryImagePersister {

		/// <summary>
		/// Gets an absolute URL to an image.
		/// </summary>
		/// <param name="picture">Image information. Cannot be null.</param>
		/// <param name="size">Image size.</param>
		/// <param name="ssl">Whether the URL should use the SSL domain.</param>
		/// <returns>Absolute URL to the image, for example "http://static.vocadb.net/img/Album/Orig/Full/123.jpg"</returns>
		string GetUrlAbsolute(IEntryImageInformation picture, ImageSize size, bool ssl);

		/// <summary>
		/// Gets stream for reading an image from the store.
		/// </summary>
		/// <param name="picture">Image information. Cannot be null.</param>
		/// <param name="size">Image size.</param>
		/// <returns>Stream for reading the image. Cannot be null.</returns>
		Stream GetReadStream(IEntryImageInformation picture, ImageSize size);

		/// <summary>
		/// Checks whether a specific image exists in the store.
		/// </summary>
		/// <param name="picture">Image information. Cannot be null.</param>
		/// <param name="size">Image size.</param>
		/// <returns>True if the image exists, otherwise false.</returns>
		bool HasImage(IEntryImageInformation picture, ImageSize size);

		/// <summary>
		/// Writes an image file stream to the store.
		/// The image will be written as is. Usually this is used for saving the original image.
		/// </summary>
		/// <param name="picture">Image information. Cannot be null.</param>
		/// <param name="size">Image size.</param>
		/// <param name="stream">Image file stream to be written. Cannot be null.</param>
		void Write(IEntryImageInformation picture, ImageSize size, Stream stream);

		/// <summary>
		/// Writes an <see cref="Image"/> to the store.
		/// </summary>
		/// <param name="picture">Image information. Cannot be null.</param>
		/// <param name="size">Image size.</param>
		/// <param name="image">Image object. Cannot be null.</param>
		void Write(IEntryImageInformation picture, ImageSize size, Image image);

	}


}
