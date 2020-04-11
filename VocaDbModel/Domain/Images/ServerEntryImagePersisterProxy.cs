using System;
using System.Linq;

namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Supports generating image URL for any type of entry supported by <see cref="IEntryImageUrlFactory"/>.
	/// Automatically chooses the approprise implementation.
	/// </summary>
	public class ServerEntryImagePersisterProxy : IEntryImageUrlFactory {

		private readonly IEntryImageUrlFactory[] factories;

		public ServerEntryImagePersisterProxy(IDynamicImageUrlFactory dynamicImageUrlFactory, IEntryThumbPersister thumbPersister, IEntryImagePersisterOld entryImagePersisterOld) {
			factories = new IEntryImageUrlFactory[] {
				dynamicImageUrlFactory,
				thumbPersister,
				entryImagePersisterOld
			};
		}

		private IEntryImageUrlFactory Choose(IEntryImageInformation imageInfo, ImageSize size) {
			return factories.FirstOrDefault(f => f.IsSupported(imageInfo, size))
				?? throw new ArgumentException("Invalid image info: " + imageInfo, nameof(imageInfo));
		}

		public string GetUrlAbsolute(IEntryImageInformation picture, ImageSize size) {
			return Choose(picture, size).GetUrlAbsolute(picture, size);
		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size) {
			return Choose(picture, size).HasImage(picture, size);
		}

		public bool IsSupported(IEntryImageInformation picture, ImageSize size) => true;

	}
}
