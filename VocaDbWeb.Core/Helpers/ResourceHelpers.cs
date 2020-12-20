#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using Microsoft.AspNetCore.Html;

namespace VocaDb.Web.Helpers
{
	public static class ResourceHelpers
	{
		public static IDictionary<object, object> ToDict(ResourceManager resourceManager)
		{
			return resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true).Cast<DictionaryEntry>().ToDictionary(k => k.Key, v => v.Value);
		}

		public static IHtmlContent ToJSON(ResourceManager resourceManager, bool lowerCase = false)
		{
			var dic = ToDict(resourceManager);

			return new HtmlString(JsonHelpers.Serialize(dic, lowerCase));
		}
	}
}