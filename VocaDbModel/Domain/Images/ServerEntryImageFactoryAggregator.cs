#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Domain.Images
{
	/// <summary>
	/// Supports generating image URL for any type of entry supported by <see cref="IEntryImageInformation"/>.
	/// Automatically chooses the appropriate implementation.
	/// Use this always when you need to generate image URLs for <see cref="IEntryImageInformation"/>.
	/// TODO: write tests for this.
	/// </summary>
	public interface IAggregatedEntryImageUrlFactory : IEntryImageUrlFactory { }

	public class ServerEntryImageFactoryAggregator : IAggregatedEntryImageUrlFactory
	{
		// TODO: optimize with lookups
		private readonly IEntryImageUrlFactory[] _factories;

		public ServerEntryImageFactoryAggregator(IDynamicImageUrlFactory dynamicImageUrlFactory, IEntryThumbPersister thumbPersister)
		{
			_factories = new IEntryImageUrlFactory[] {
				thumbPersister,
				dynamicImageUrlFactory
			};
		}

		private IEnumerable<IEntryImageUrlFactory> Factories(IEntryImageInformation imageInfo, ImageSize size) =>
			_factories.Where(f => f.IsSupported(imageInfo, size));

		private IEnumerable<IEntryImageUrlFactory> FactoriesCheckExist(IEntryImageInformation imageInfo, ImageSize size) =>
			Factories(imageInfo, size).Where(f => f.HasImage(imageInfo, size));

		public VocaDbUrl GetUrl(IEntryImageInformation imageInfo, ImageSize size)
		{
			// Logic: try to get URL from source where it exists.
			// If it seems the file doesn't exist, try a secondary source and finally fall back to accepting source where the file doesn't exist.
			return FactoriesCheckExist(imageInfo, size).Select(f => f.GetUrl(imageInfo, size)).FirstOrDefault()
				?? Factories(imageInfo, size).Select(f => f.GetUrl(imageInfo, size)).FirstOrDefault()
				?? throw new ArgumentException($"Could not find URL factory for {imageInfo}", nameof(imageInfo));
		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size)
		{
			return Factories(picture, size).Any(f => f.HasImage(picture, size));
		}

		public bool IsSupported(IEntryImageInformation picture, ImageSize size) => Factories(picture, size).Any();
	}
}
