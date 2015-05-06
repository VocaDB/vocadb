using System;
using System.Drawing;
using System.IO;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Images {

	public class ImageThumbGenerator {

		private readonly IEntryImagePersister persister;

		public const int Unlimited = 0;

		public ImageThumbGenerator(IEntryImagePersister persister) {
			
			ParamIs.NotNull(() => persister);

			this.persister = persister;

		}

		/// <summary>
		/// Writes an image to a file, overwriting any existing file.
		/// 
		/// If the dimensions of the original image are smaller or equal than the thumbnail size,
		/// the file is simply copied. Otherwise it will be shrunk.
		/// </summary>
		/// <param name="original">Original image. Cannot be null.</param>
		/// <param name="input">Stream to be written. Cannot be null.</param>
		/// <param name="imageInfo">Image information. Cannot be null.</param>
		/// <param name="size">Image size of the saved thumbnail.</param>
		/// <param name="dimensions">Dimensions of the thumbnail.</param>
		private void GenerateThumbAndMoveImage(Image original, Stream input, IEntryImageInformation imageInfo, ImageSize size, int dimensions) {

			if (dimensions != Unlimited && (original.Width > dimensions || original.Height > dimensions)) {
				using (var thumb = ImageHelper.ResizeToFixedSize(original, dimensions, dimensions)) {
					persister.Write(imageInfo, size, thumb);
				}
			} else {
				persister.Write(imageInfo, size, input);
			}

		}

		/// <summary>
		/// Generates thumbnails and writes the original file into external image files.
		/// </summary>
		/// <exception cref="InvalidPictureException">If the image could not be opened. Most likely the file is broken.</exception>
		public void GenerateThumbsAndMoveImage(Stream input, IEntryImageInformation imageInfo, ImageSizes imageSizes, int originalSize = Unlimited) {

			using (var original = ImageHelper.OpenImage(input)) {

				if (imageSizes.HasFlag(ImageSizes.Original))
					GenerateThumbAndMoveImage(original, input, imageInfo, ImageSize.Original, originalSize);

				if (imageSizes.HasFlag(ImageSizes.Thumb))
					GenerateThumbAndMoveImage(original, input, imageInfo, ImageSize.Thumb, ImageHelper.DefaultThumbSize);

				if (imageSizes.HasFlag(ImageSizes.SmallThumb))
					GenerateThumbAndMoveImage(original, input, imageInfo, ImageSize.SmallThumb, ImageHelper.DefaultSmallThumbSize);

				if (imageSizes.HasFlag(ImageSizes.TinyThumb))
					GenerateThumbAndMoveImage(original, input, imageInfo, ImageSize.TinyThumb, ImageHelper.DefaultTinyThumbSize);

			}

		}

	}

	public enum ImageSize {

		/// <summary>
		/// Original image. 
		/// Typically full size, although a maximum size can be placed as well.
		/// </summary>
		Original = 1,

		/// <summary>
		/// Large thumbnail, default size is 250x250
		/// </summary>
		Thumb = 2,

		/// <summary>
		/// Small thumbnail, default size is 150x150
		/// </summary>
		SmallThumb = 4,

		/// <summary>
		/// Tiny thumbnail, default size is 70x70
		/// </summary>
		TinyThumb = 8,

	}

	[Flags]
	public enum ImageSizes {

		Nothing = 0,

		Original = 1,

		Thumb = 2,

		SmallThumb = 4,

		TinyThumb = 8,

		All = (Original | Thumb | SmallThumb | TinyThumb)

	}

}
