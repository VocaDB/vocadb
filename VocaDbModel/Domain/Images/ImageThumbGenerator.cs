using NLog;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Images;

public class ImageThumbGenerator
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

	private readonly IEntryImagePersister _persister;

	public const int Unlimited = ImageHelper.ImageSizeUnlimited;

	public ImageThumbGenerator(IEntryImagePersister persister)
	{
		ParamIs.NotNull(() => persister);

		_persister = persister;
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
	private void GenerateThumbAndMoveImage(Image original, Stream input, IEntryImageInformation imageInfo, ImageSize size, int dimensions)
	{
		if (dimensions != Unlimited && (original.Width > dimensions || original.Height > dimensions))
		{
			using var thumb = ImageHelper.ResizeToFixedSize(original, dimensions, dimensions);

			_persister.Write(imageInfo, size, thumb);
		}
		else
		{
			_persister.Write(imageInfo, size, input);
		}
	}

	/// <summary>
	/// Generates thumbnails and writes the original file into external image files.
	/// </summary>
	/// <exception cref="InvalidPictureException">If the image could not be opened. Most likely the file is broken.</exception>
	public void GenerateThumbsAndMoveImage(Stream input, IEntryImageInformation imageInfo, ImageSizes imageSizes, int originalSize = Unlimited)
	{
		using var original = ImageHelper.OpenImage(input);

		if (imageSizes.HasFlag(ImageSizes.Original))
			GenerateThumbAndMoveImage(original, input, imageInfo, ImageSize.Original, originalSize);

		if (imageSizes.HasFlag(ImageSizes.Thumb))
			GenerateThumbAndMoveImage(original, input, imageInfo, ImageSize.Thumb, ImageHelper.DefaultThumbSize);

		if (imageSizes.HasFlag(ImageSizes.SmallThumb))
			GenerateThumbAndMoveImage(original, input, imageInfo, ImageSize.SmallThumb, ImageHelper.DefaultSmallThumbSize);

		if (imageSizes.HasFlag(ImageSizes.TinyThumb))
			GenerateThumbAndMoveImage(original, input, imageInfo, ImageSize.TinyThumb, ImageHelper.DefaultTinyThumbSize);
	}

	/// <summary>
	/// Generates the thumbnails that are missing from the store.
	/// </summary>
	/// <returns>False if the original isn't in the store either.</returns>
	public bool GenerateMissingThumbs(IEntryImageInformation imageInfo)
	{
		if (!_persister.Exists(imageInfo, ImageSize.Original))
			return false;

		var missing = ImageSizes.Nothing;

		if (!_persister.Exists(imageInfo, ImageSize.Thumb))
			missing |= ImageSizes.Thumb;

		if (!_persister.Exists(imageInfo, ImageSize.SmallThumb))
			missing |= ImageSizes.SmallThumb;

		if (!_persister.Exists(imageInfo, ImageSize.TinyThumb))
			missing |= ImageSizes.TinyThumb;

		if (missing == ImageSizes.Nothing)
			return true;

		using var original = _persister.GetReadStream(imageInfo, ImageSize.Original);
		GenerateThumbsAndMoveImage(original, imageInfo, missing);
		return true;
	}

	/// <summary>
	/// Restores the main picture of an archived version when reverting to it.
	/// A picture without revision is uploaded from the database to the revision of the archived version.
	/// </summary>
	/// <returns>Picture revision for the entry.</returns>
	public int? RestoreMainPicture(IEntryBase entry, string? mime, int? revision, int archivedVersion, Func<byte[]?> loadBytes)
	{
		if (string.IsNullOrEmpty(mime))
			return null;

		if (revision is int rev && GenerateMissingThumbs(new EntryThumbMain(entry, mime, rev)))
			return rev;

		var bytes = loadBytes();

		if (bytes == null)
		{
			if (revision != null)
				s_log.Warn("Picture revision {0} of {1} is missing from the image store, and there is no copy in the database", revision, entry);

			return revision;
		}

		var newRevision = revision ?? archivedVersion;
		using var stream = new MemoryStream(bytes);
		GenerateThumbsAndMoveImage(stream, new EntryThumbMain(entry, mime, newRevision), ImageSizes.All);
		return newRevision;
	}
}
