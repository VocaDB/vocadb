using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;
using VocaDb.Model.Domain;
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

		public VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size) {
			// For testing it's really doesn't matter if it's absolute or relative, it just needs to be unique.
			return new VocaDbUrl(picture.EntryType + "/" + picture.Id + "/" + size, UrlDomain.Main, System.UriKind.Absolute);
		}

		public Stream GetReadStream(IEntryImageInformation picture, ImageSize size) {
			return new MemoryStream(images[GetUrl(picture, size).Url]);
		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size) {
			return images.ContainsKey(GetUrl(picture, size).Url);
		}

		public void Write(IEntryImageInformation picture, ImageSize size, Stream stream) {

			var bytes = StreamHelper.ReadStream(stream);

			var url = GetUrl(picture, size).Url;

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
