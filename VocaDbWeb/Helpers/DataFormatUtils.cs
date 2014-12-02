using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace VocaDb.Web.Helpers {

	public static class DataFormatUtils {

		public static IEnumerable<MvcHtmlString> GenerateHtml<T>(IEnumerable<T> source, Func<T, MvcHtmlString> transform) {

			if (source == null)
				return null;

			return source.Select(transform);

		}

		public static IEnumerable<MvcHtmlString> GenerateHtmlFromHelper<T>(IEnumerable<T> source, Func<T, HelperResult> transform) {

			if (source == null)
				return null;

			return source.Select(s => MvcHtmlString.Create(transform(s).ToHtmlString()));

		}

	}

}