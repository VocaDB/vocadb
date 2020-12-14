#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VocaDb.Web.Helpers
{
	public static class StringHelper
	{
		public static IHtmlString Join(string separator, IEnumerable<IHtmlString> strings)
		{
			return new MvcHtmlString(string.Join(separator, strings.Select(s => s.ToHtmlString())));
		}
	}
}