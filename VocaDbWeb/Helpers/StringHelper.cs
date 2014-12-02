using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace VocaDb.Web.Helpers {

	public static class StringHelper {

		public static MvcHtmlString Join(string separator, IEnumerable<MvcHtmlString> strings) {

			return new MvcHtmlString(string.Join(separator, strings.Select(s => s.ToHtmlString())));

			/*if (!strings.Any())
				return MvcHtmlString.Empty;

			var s = strings.Aggregate(new StringBuilder(), (sb, str) => sb.Append(str.ToHtmlString() 
				+ (str != strings.Last() ? ", " : string.Empty)));

			return new MvcHtmlString(s.ToString());*/

		}

	}

}