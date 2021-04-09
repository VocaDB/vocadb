using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;

namespace VocaDb.Web.Helpers
{
	public static class StringHelper
	{
		public static IHtmlContent Join(string? separator, IEnumerable<IHtmlContent> strings)
		{
			return new HtmlString(string.Join(separator, strings.Select(s => s.ToHtmlString())));
		}
	}
}