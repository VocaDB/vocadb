using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VocaDb.Web.Helpers {

	public static class StringHelper {

		public static MvcHtmlString Join(string separator, IEnumerable<MvcHtmlString> strings) {

			return new MvcHtmlString(string.Join(separator, strings.Select(s => s.ToHtmlString())));

		}

		public static MvcHtmlString Join(string separator, IEnumerable<IHtmlString> strings) {

			return new MvcHtmlString(string.Join(separator, strings.Select(s => s.ToHtmlString())));

		}

	}

}