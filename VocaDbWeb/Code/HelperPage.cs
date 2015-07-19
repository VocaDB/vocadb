using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;
using VocaDb.Model.Service;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code {

	public class HelperPage : System.Web.WebPages.HelperPage {

		public static IEntryLinkFactory EntryLinkFactory {
			get {
				return (IEntryLinkFactory)DependencyResolver.Current.GetService(typeof(IEntryLinkFactory));
			}
		}

		// Workaround - exposes the MVC HtmlHelper instead of the normal helper
		public static new HtmlHelper Html {
			get { return ((WebViewPage)WebPageContext.Current.Page).Html; }
		}

		public static MarkdownParser MarkdownParser {
			get { return DependencyResolver.Current.GetService<MarkdownParser>(); }
		}

		public static string RequestUrlScheme {
			get {
				return WebHelper.IsSSL(Request) ? "https" : "http";
			}
		}

		public static IHtmlString ToJS(string str) {
			return new HtmlString(JsonHelpers.Serialize(str));
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
