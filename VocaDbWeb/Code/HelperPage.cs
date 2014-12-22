using System.Web.WebPages;
using System.Web.Mvc;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code {

	public class HelperPage : System.Web.WebPages.HelperPage {

		// Workaround - exposes the MVC HtmlHelper instead of the normal helper
		public static new HtmlHelper Html {
			get { return ((WebViewPage)WebPageContext.Current.Page).Html; }
		}

		public static string RequestUrlScheme {
			get {
				return WebHelper.IsSSL(Request) ? "https" : "http";
			}
		}

		public static UrlHelper Url {
			get { return ((WebViewPage)WebPageContext.Current.Page).Url; }
		}

		public static ViewDataDictionary ViewData {
			get {
				return ((WebViewPage)WebPageContext.Current.Page).ViewData; 
			}
		}

	}
}
