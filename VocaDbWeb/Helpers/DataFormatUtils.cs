using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace VocaDb.Web.Helpers {

	public static class DataFormatUtils {

		public static IEnumerable<MvcHtmlString> GenerateHtml<T>(IEnumerable<T> source, Func<T, MvcHtmlString> transform) {

			if (source == null)
				return null;

			return source.Select(transform);

		}

	}

}