using System.Collections.Generic;

namespace VocaDb.Model.EmbeddedResources {

	public static class EmbeddedResourceManager {

		public static IDictionary<string, string> NicoTagMapping {
			get {
				return EmbeddedResourceHelper.ReadJson<IDictionary<string, string>>("nicoTagMapping.json");
			}
		}

	}

}
