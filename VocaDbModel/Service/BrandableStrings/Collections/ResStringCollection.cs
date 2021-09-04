using System.Resources;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.BrandableStrings.Collections
{
	public abstract class ResStringCollection
	{
		protected ResStringCollection(ResourceManager resourceManager)
		{
			ResourceManager = resourceManager;
		}

		public ResourceManager ResourceManager { get; }

		protected string? GetString(string name) => ResourceManager.GetString(name);

		protected string GetString(string name, string fallback) => GetString(name).EmptyToNull() ?? fallback;
	}
}
