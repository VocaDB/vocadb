using System;

namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Supports generating image URL for any type of entry supported by <see cref="IEntryImageUrlFactory"/>.
	/// Automatically chooses the approprise implementation.
	/// </summary>
	public class ServerEntryImagePersisterProxy : IEntryImageUrlFactory {

		private readonly IDynamicImageUrlFactory dynamicImageUrlFactory;
		private readonly IEntryThumbPersister thumbPersister;
		private readonly IEntryImagePersisterOld entryImagePersisterOld;

		public ServerEntryImagePersisterProxy(IDynamicImageUrlFactory dynamicImageUrlFactory, IEntryThumbPersister thumbPersister, IEntryImagePersisterOld entryImagePersisterOld) {
			this.dynamicImageUrlFactory = dynamicImageUrlFactory;
			this.thumbPersister = thumbPersister;
			this.entryImagePersisterOld = entryImagePersisterOld;
		}

		private IEntryImageUrlFactory Choose(IEntryImageInformation imageInfo, ImageSize size) {
			if (dynamicImageUrlFactory.IsSupported(imageInfo, size))
				return dynamicImageUrlFactory;
			if (thumbPersister.IsSupported(imageInfo, size))
				return thumbPersister;
			if (entryImagePersisterOld.IsSupported(imageInfo, size))
				return entryImagePersisterOld;
			throw new ArgumentException("Invalid image info: " + imageInfo, nameof(imageInfo));
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
