#nullable disable

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net.Mime;
using System.Runtime.Serialization;
using NLog;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.Helpers;

/// <summary>
/// Various image helper methods.
/// </summary>
public static class ImageHelper
{
#nullable enable
	private static readonly string[] s_allowedExt = { ".bmp", ".gif", ".jpg", ".jpeg", ".png" };
	public const int DefaultSmallThumbSize = 150;
	public const int DefaultThumbSize = 250;
	public const int DefaultTinyThumbSize = 70;
	public const int UserThumbMax = 512;
	public const int UserThumbSize = 80;
	public const int UserSmallThumbSize = 40;
	public const int UserTinyThumbSize = 20;
	public const int ImageSizeUnlimited = 0;
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

	/// <summary>
	/// Opens image and logs an exception if the image cannot be opened.
	/// </summary>
	/// <param name="stream">File stream. Cannot be null.</param>
	/// <returns>Opened image which must be disposed after using. Cannot be null.</returns>
	/// <exception cref="InvalidPictureException">If the image could not be opened. Most likely the file is broken.</exception>
	public static Image OpenImage(Stream stream)
	{
		try
		{
			return Image.FromStream(stream);
		}
		catch (ArgumentException x)
		{
			s_log.Error(x, "Unable to open image");
			throw new InvalidPictureException("Unable to open image", x);
		}
	}

	public const int MaxImageSizeMB = 8;
	public const int MaxImageSizeBytes = MaxImageSizeMB * 1024 * 1024;

	public static string[] AllowedExtensions => s_allowedExt;

	/// <summary>
	/// Gets image extension from MIME type.
	/// </summary>
	/// <param name="mime">MIME type. Can be null or empty.</param>
	/// <returns>File extension, for example ".jpg". Can be null if MIME type is not recognized.</returns>
	public static string GetExtensionFromMime(string? mime) => mime switch
	{
		MediaTypeNames.Image.Jpeg => ".jpg",
		"image/pjpeg" => ".jpg",
		"image/png" => ".png",
		MediaTypeNames.Image.Gif => ".gif",
		"image/bmp" => ".bmp",
		"image/x-ms-bmp" => ".bmp",
		"audio/mp3" or "audio/mpeg" => ".mp3",
		_ => string.Empty,
	};

	public static int GetDefaultImageSizePx(ImageSize size) => size switch
	{
		ImageSize.Thumb => DefaultThumbSize,
		ImageSize.SmallThumb => DefaultSmallThumbSize,
		ImageSize.TinyThumb => DefaultTinyThumbSize,
		_ => ImageSizeUnlimited,
	};

	/// <summary>
	/// Gets the size in pixels for user's profile picture.
	/// </summary>
	public static int GetUserImageSizePx(ImageSize size) => size switch
	{
		ImageSize.Original => UserThumbMax,
		ImageSize.Thumb or ImageSize.SmallThumb => UserThumbSize,
		ImageSize.TinyThumb => UserTinyThumbSize,
		_ => UserThumbMax,
	};
#nullable disable

	public static PictureDataContract GetOriginal(Stream input, int length, string contentType)
	{
		var buf = new Byte[length];
		input.Read(buf, 0, length);

		return new PictureDataContract(buf, contentType);
	}

#nullable enable
	public static bool IsValidImageExtension(string? fileName)
	{
		var ext = Path.GetExtension(fileName);

		return (s_allowedExt.Any(e => string.Equals(e, ext, StringComparison.InvariantCultureIgnoreCase)));
	}

	public static Image ResizeToFixedSize(Image imgPhoto, int width, int height)
	{
		int sourceWidth = imgPhoto.Width;
		int sourceHeight = imgPhoto.Height;

		double nPercent;
		var nPercentW = ((double)width / (double)sourceWidth);
		var nPercentH = ((double)height / (double)sourceHeight);

		if (nPercentH < nPercentW)
		{
			nPercent = nPercentH;
		}
		else
		{
			nPercent = nPercentW;
		}

		int destWidth = width = (int)(sourceWidth * nPercent);
		int destHeight = height = (int)(sourceHeight * nPercent);

		var bmPhoto = new Bitmap(width, height,
						  PixelFormat.Format32bppArgb);
		bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
						 imgPhoto.VerticalResolution);

		using (var grPhoto = Graphics.FromImage(bmPhoto))
		{
			grPhoto.Clear(Color.Transparent);
			grPhoto.InterpolationMode =
					InterpolationMode.HighQualityBicubic;

			grPhoto.DrawImage(imgPhoto,
				new Rectangle(0, 0, destWidth, destHeight),
				new Rectangle(0, 0, sourceWidth, sourceHeight),
				GraphicsUnit.Pixel);
		}

		return bmPhoto;
	}
#nullable disable
}

/// <summary>
/// Exception thrown if there was an error while opening a picture file.
/// </summary>
public class InvalidPictureException : Exception
{
#nullable enable
	public InvalidPictureException() { }
	public InvalidPictureException(string? message) : base(message) { }
	public InvalidPictureException(string? message, Exception? innerException) : base(message, innerException) { }
	protected InvalidPictureException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#nullable disable
}
