using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code {

	public class HelperPage : System.Web.WebPages.HelperPage {

		public static IEntryLinkFactory EntryLinkFactory => (IEntryLinkFactory)DependencyResolver.Current.GetService(typeof(IEntryLinkFactory));

		// Workaround - exposes the MVC HtmlHelper instead of the normal helper
		public static new HtmlHelper Html => ((WebViewPage)WebPageContext.Current.Page).Html;

		public static MarkdownParser MarkdownParser => DependencyResolver.Current.GetService<MarkdownParser>();

		public static string RequestUrlScheme => WebHelper.IsSSL(Request) ? "https" : "http";

		public static IHtmlString ToJS(string str) {
			return new HtmlString(JsonHelpers.Serialize(str));
		}

		public static UrlHelper Url => ((WebViewPage)WebPageContext.Current.Page).Url;

		public static VocaUrlMapper UrlMapper => new VocaUrlMapper(WebHelper.IsSSL(Request));

		public static ViewDataDictionary ViewData => ((WebViewPage)WebPageContext.Current.Page).ViewData;

	}
}
