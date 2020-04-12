using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.TestSupport {

	/// <summary>
	/// In-memory image persisting system (virtual filesystem), for testing.
	/// </summary>
	public class InMemoryImagePersister : IAggregatedEntryImageUrlFactory, IEntryImagePersisterOld, IEntryThumbPersister, IEntryPictureFilePersister {

		private readonly Dictionary<string, byte[]> images = new Dictionary<string, byte[]>();

		private ImageFormat GetImageFormat(IEntryImageInformation imageInfo) {
			switch (imageInfo.Mime) {
				case MediaTypeNames.Image.Jpeg:
					return ImageFormat.Jpeg;
				case "image/png":
					return ImageFormat.Png;
				case MediaTypeNames.Image.Gif:
					return ImageFormat.Gif;
				case "image/bmp":
					return ImageFormat.Bmp;
				default:
					return ImageFormat.Png;
			}
		}

		public string GetUrlAbsolute(IEntryImageInformation picture, ImageSize size) {
			return picture.EntryType + "/" + picture.Id + "/" + size;
		}

		public Stream GetReadStream(IEntryImageInformation picture, ImageSize size) {
			return new MemoryStream(images[GetUrlAbsolute(picture, size)]);
		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size) {
			return images.ContainsKey(GetUrlAbsolute(picture, size));
		}

		public void Write(IEntryImageInformation picture, ImageSize size, Stream stream) {

			var bytes = StreamHelper.ReadStream(stream);

			var url = GetUrlAbsolute(picture, size);

			if (images.ContainsKey(url))
				images[url] = bytes;
			else
				images.Add(url, bytes);

		}

		public void Write(IEntryImageInformation picture, ImageSize size, Image image) {

			using (var stream = new MemoryStream()) {
				image.Save(stream, GetImageFormat(picture));
				Write(picture, size, stream);
			}

		}

		public bool IsSupported(IEntryImageInformation picture, ImageSize size) {
			return true;
		}
	}

}
