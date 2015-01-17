using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Net.Mime;
using System.Runtime.Serialization;
using NLog;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.Helpers {

	/// <summary>
	/// Various image helper methods.
	/// TODO: need figure out which ones these are still in use.
	/// </summary>
	public static class ImageHelper {

		private static readonly string[] allowedExt = { ".bmp", ".gif", ".jpg", ".jpeg", ".png" };
		public const int DefaultSmallThumbSize = 150;
		public const int DefaultThumbSize = 250;
		public const int DefaultTinyThumbSize = 70;
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public static Image OpenImage(Stream stream) {
			try {
				return Image.FromStream(stream);
			} catch (ArgumentException x) {
				log.Error("Unable to open image", x);
				throw new InvalidPictureException("Unable to open image", x);
			}
		}

		public const int MaxImageSizeMB = 8;
		public const int MaxImageSizeBytes = MaxImageSizeMB * 1024 * 1024;

		public static string[] AllowedExtensions {
			get { return allowedExt; }
		}

		// Used for persisting album and artist additional pictures. TODO: this should be refactored to ImageThumbGenerator
		public static void GenerateThumbsAndMoveImages(IEnumerable<EntryPictureFile> newPictures) {

			foreach (var pic in newPictures) {

				if (pic.UploadedFile == null)
					continue;

				var path = GetImagePath(pic);
				var thumbPath = GetImagePathThumb(pic);

				using (var f = File.Create(path)) {
					pic.UploadedFile.CopyTo(f);
				}
				pic.UploadedFile.Seek(0, SeekOrigin.Begin);

				using (var original = OpenImage(pic.UploadedFile)) {

					if (original.Width > DefaultThumbSize || original.Height > DefaultThumbSize) {
						using (var thumb = ResizeToFixedSize(original, DefaultThumbSize, DefaultThumbSize)) {
							thumb.Save(thumbPath);							
						}
					} else {
						File.Copy(path, thumbPath);
					}

				}

			}

		}

		/// <summary>
		/// Gets image extension from MIME type.
		/// </summary>
		/// <param name="mime">MIME type. Can be null or empty.</param>
		/// <returns>File extension, for example ".jpg". Can be null if MIME type is not recognized.</returns>
		public static string GetExtensionFromMime(string mime) {

			switch (mime) {
				case MediaTypeNames.Image.Jpeg:
					return ".jpg";
				case "image/pjpeg":
					return ".jpg";
				case "image/png":
					return ".png";
				case MediaTypeNames.Image.Gif:
					return ".gif";
				case "image/bmp":
					return ".bmp";
				case "image/x-ms-bmp":
					return ".bmp";
				default:
					return string.Empty;
			}

		}

		private static string GetImagePath(EntryPictureFile picture) {
			return new ServerEntryImagePersisterOld().GetPath(picture, ImageSize.Original);
		}

		private static string GetImagePathThumb(EntryPictureFile picture) {
			return new ServerEntryImagePersisterOld().GetPath(picture, ImageSize.Thumb);
		}

		public static PictureDataContract GetOriginal(Stream input, int length, string contentType) {

			var buf = new Byte[length];
			input.Read(buf, 0, length);

			return new PictureDataContract(buf, contentType);

		}

		public static bool IsValidImageExtension(string fileName) {

			var ext = Path.GetExtension(fileName);

			return (allowedExt.Any(e => string.Equals(e, ext, StringComparison.InvariantCultureIgnoreCase)));

		}

		public static Image ResizeToFixedSize(Image imgPhoto, int width, int height) {

			int sourceWidth = imgPhoto.Width;
			int sourceHeight = imgPhoto.Height;

			double nPercent;
			var nPercentW = ((double)width / (double)sourceWidth);
			var nPercentH = ((double)height / (double)sourceHeight);

			if (nPercentH < nPercentW) {
				nPercent = nPercentH;
			} else {
				nPercent = nPercentW;
			}

			int destWidth = width = (int)(sourceWidth * nPercent);
			int destHeight = height = (int)(sourceHeight * nPercent);

			var bmPhoto = new Bitmap(width, height,
							  PixelFormat.Format32bppArgb);
			bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
							 imgPhoto.VerticalResolution);

			using (var grPhoto = Graphics.FromImage(bmPhoto)) {

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
	}

	public class InvalidPictureException : Exception {
		public InvalidPictureException() {}
		public InvalidPictureException(string message) : base(message) {}
		public InvalidPictureException(string message, Exception innerException) : base(message, innerException) {}
		protected InvalidPictureException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}

}
