#nullable disable

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.TestSupport
{
	/// <summary>
	/// In-memory image persisting system (virtual filesystem), for testing.
	/// </summary>
	public class InMemoryImagePersisterStore
	{
		private readonly Dictionary<string, byte[]> images = new Dictionary<string, byte[]>();

		private ImageFormat GetImageFormat(IEntryImageInformation imageInfo) => imageInfo.Mime switch
		{
			MediaTypeNames.Image.Jpeg => ImageFormat.Jpeg,
			"image/png" => ImageFormat.Png,
			MediaTypeNames.Image.Gif => ImageFormat.Gif,
			"image/bmp" => ImageFormat.Bmp,
			_ => ImageFormat.Png,
		};

		public VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size)
		{
			// For testing it's really doesn't matter if it's absolute or relative, it just needs to be unique.
			return new VocaDbUrl($"https://static.vocadb.net/img/{picture.EntryType}/{picture.Id}/{size}", UrlDomain.Main, System.UriKind.Absolute);
		}

		public Stream GetReadStream(IEntryImageInformation picture, ImageSize size)
		{
			return new MemoryStream(images[GetUrl(picture, size).Url]);
		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size)
		{
			return images.ContainsKey(GetUrl(picture, size).Url);
		}

		public void Write(IEntryImageInformation picture, ImageSize size, Stream stream)
		{
			var bytes = StreamHelper.ReadStream(stream);

			var url = GetUrl(picture, size).Url;
			images[url] = bytes;
		}

		public void Write(IEntryImageInformation picture, ImageSize size, Image image)
		{
			using (var stream = new MemoryStream())
			{
				image.Save(stream, GetImageFormat(picture));
				Write(picture, size, stream);
			}
		}
	}

	public abstract class InMemoryImagePersisterBase : IEntryImagePersister
	{
		public InMemoryImagePersisterBase() : this(new InMemoryImagePersisterStore()) { }

		public InMemoryImagePersisterBase(InMemoryImagePersisterStore store)
		{
			this.store = store;
		}

		private readonly InMemoryImagePersisterStore store;

		public Stream GetReadStream(IEntryImageInformation picture, ImageSize size) => store.GetReadStream(picture, size);

		public void Write(IEntryImageInformation picture, ImageSize size, Stream stream) => store.Write(picture, size, stream);

		public void Write(IEntryImageInformation picture, ImageSize size, Image image) => store.Write(picture, size, image);

		public VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size) => store.GetUrl(picture, size);

		public bool HasImage(IEntryImageInformation picture, ImageSize size) => store.HasImage(picture, size);

		public abstract bool IsSupported(IEntryImageInformation picture, ImageSize size);
	}

	public class InMemoryImagePersister : InMemoryImagePersisterBase, IAggregatedEntryImageUrlFactory, IEntryThumbPersister, IEntryPictureFilePersister
	{
		public override bool IsSupported(IEntryImageInformation picture, ImageSize size) => true;
	}

	public class InMemoryEntryThumbPersister : InMemoryImagePersisterBase, IEntryThumbPersister
	{
		public InMemoryEntryThumbPersister(InMemoryImagePersisterStore store) : base(store) { }

		public override bool IsSupported(IEntryImageInformation picture, ImageSize size)
		{
			return new ServerEntryThumbPersister().IsSupported(picture, size);
		}
	}
}
