using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.EmbeddedResources;

namespace VocaDb.Model.Service.VideoServices {

	public class NicoTagMappingFactory {

		private readonly ObjectCache cache;
		private const string cacheKey = "NicoTagMapping";

		public NicoTagMappingFactory(ObjectCache cache) {
			this.cache = cache;
		}

		public Dictionary<string, string> GetMappings() {
			
			return cache.GetOrInsert(cacheKey, CachePolicy.AbsoluteExpiration(24), () => {
				
				var mappings = EmbeddedResourceManager.NicoTagMapping;
				return mappings.ToDictionary(m => m.Key, m => m.Value, StringComparer.InvariantCultureIgnoreCase);

			});

		}

	}

}
