using System.Drawing;
using System.IO;

namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Writes entry images to a store (such as disk) and loads them back as needed.
	/// </summary>
	public interface IEntryImagePersister {

		/// <summary>
		/// Gets an absolute URL for internal and external access to an image.
		/// </summary>
		/// <param name="picture">Image information. Cannot be null.</param>
		/// <param name="size">Image size.</param>
		/// <returns>
		/// Absolute URL to the image, for example "https://static.vocadb.net/img/Album/Orig/Full/123.jpg".
		/// Cannot be null, but not guaranteed to exist (use <see cref="HasImage"/> to make sure file exists).
		/// </returns>
		string GetUrlAbsolute(IEntryImageInformation picture, ImageSize size);

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

	/// <summary>
	/// Extensions to <see cref="IEntryImagePersister"/>.
	/// </summary>
	public static class IEntryImagePersisterExtender {

		/// <summary>
		/// Gets absolute URL to image, optionally verifying that it exists.
		/// </summary>
		/// <param name="persister">Persister. Cannot be null.</param>
		/// <param name="picture">Image information. Cannot be null.</param>
		/// <param name="size">Image size.</param>
		/// <param name="checkExists">If true, verify that the image exists. If false, no verification is done.</param>
		/// <returns>
		/// Absolute URL to image, if image exists or <paramref name="checkExists"/> is false. 
		/// If <paramref name="checkExists"/> is true and image does not exist, this will be null.
		/// </returns>
		public static string GetUrlAbsolute(this IEntryImagePersister persister, IEntryImageInformation picture, ImageSize size, bool checkExists) {

			if (checkExists && !persister.HasImage(picture, size))
				return null;

			return persister.GetUrlAbsolute(picture, size);

		}

	}

}
