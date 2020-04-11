using System;
using System.Linq;

namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Supports generating image URL for any type of entry supported by <see cref="IEntryImageInformation"/>.
	/// Automatically chooses the appropriate implementation.
	/// Use this always when you need to generate image URLs for <see cref="IEntryImageInformation"/>.
	/// </summary>
	public interface IAggregatedEntryImageUrlFactory : IEntryImageUrlFactory { }

	public class ServerEntryImageFactoryAggregator : IAggregatedEntryImageUrlFactory {

		// TODO: optimize with lookups
		private readonly IEntryImageUrlFactory[] factories;

		public ServerEntryImageFactoryAggregator(IDynamicImageUrlFactory dynamicImageUrlFactory, IEntryThumbPersister thumbPersister, IEntryImagePersisterOld entryImagePersisterOld) {
			factories = new IEntryImageUrlFactory[] {
				dynamicImageUrlFactory,
				thumbPersister,
				entryImagePersisterOld
			};
		}

		private IEntryImageUrlFactory Choose(IEntryImageInformation imageInfo, ImageSize size) {
			return factories.FirstOrDefault(f => f.IsSupported(imageInfo, size))
				?? throw new ArgumentException($"Could not find URL factory for {imageInfo}", nameof(imageInfo));
		}

		public string GetUrlAbsolute(IEntryImageInformation picture, ImageSize size) {
			return Choose(picture, size).GetUrlAbsolute(picture, size);
		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size) {
			return Choose(picture, size).HasImage(picture, size);
		}

		public bool IsSupported(IEntryImageInformation picture, ImageSize size) => factories.Any(f => f.IsSupported(picture, size));

	}
}
