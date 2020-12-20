#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;

namespace VocaDb.Web.Helpers
{
	public static class DataFormatUtils
	{
		public static IEnumerable<IHtmlContent> GenerateHtml<T>(IEnumerable<T> source, Func<T, IHtmlContent> transform)
		{
			if (source == null)
				return null;

			return source.Select(transform);
		}
	}
}