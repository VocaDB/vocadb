using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Helpers;

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

		private IEnumerable<IEntryImageUrlFactory> Factories(IEntryImageInformation imageInfo, ImageSize size) =>
			factories.Where(f => f.IsSupported(imageInfo, size));

		public VocaDbUrl GetUrl(IEntryImageInformation imageInfo, ImageSize size) {
			return Factories(imageInfo, size)
				.Select(f => f.GetUrl(imageInfo, size))
				.Where(u => !u.IsEmpty)
				.FirstOrDefault()
				?? throw new ArgumentException($"Could not find URL factory for {imageInfo}", nameof(imageInfo));
		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size) {
			return Factories(picture, size).Any(f => f.HasImage(picture, size));
		}

		public bool IsSupported(IEntryImageInformation picture, ImageSize size) => Factories(picture, size).Any();

	}
}
