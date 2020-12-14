#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Mvc;

namespace VocaDb.Web.Helpers
{
	public static class ResourceHelpers
	{
		public static IDictionary<object, object> ToDict(ResourceManager resourceManager)
		{
			return resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true).Cast<DictionaryEntry>().ToDictionary(k => k.Key, v => v.Value);
		}

		public static IHtmlString ToJSON(ResourceManager resourceManager, bool lowerCase = false)
		{
			var dic = ToDict(resourceManager);

			return new MvcHtmlString(JsonHelpers.Serialize(dic, lowerCase));
		}
	}
}