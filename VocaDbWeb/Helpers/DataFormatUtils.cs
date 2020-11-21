using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VocaDb.Web.Helpers {

	public static class DataFormatUtils {

		public static IEnumerable<IHtmlString> GenerateHtml<T>(IEnumerable<T> source, Func<T, IHtmlString> transform) {

			if (source == null)
				return null;

			return source.Select(transform);

		}

	}

}