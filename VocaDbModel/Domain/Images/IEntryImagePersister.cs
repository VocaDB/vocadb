using System.Drawing;
using System.IO;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Images
{
	/// <summary>
	/// Writes entry images to a store (such as disk) and loads them back as needed.
	/// </summary>
	public interface IEntryImagePersister : IEntryImageUrlFactory
	{
		/// <summary>
		/// Gets stream for reading an image from the store.
		/// </summary>
		/// <param name="picture">Image information. Cannot be null.</param>
		/// <param name="size">Image size.</param>
		/// <returns>Stream for reading the image. Cannot be null.</returns>
		Stream GetReadStream(IEntryImageInformation picture, ImageSize size);

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

	public static class EntryImagePersisterExtensions
	{
		public static byte[] ReadBytes(this IEntryImagePersister persister, IEntryImageInformation imageInfo, ImageSize size)
		{
			using (var stream = persister.GetReadStream(imageInfo, size))
			{
				return StreamHelper.ReadStream(stream);
			}
		}
	}
}
