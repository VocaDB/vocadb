#nullable disable

using System.Web;
using System.Web.WebPages;
using System.Web.Mvc;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code
{
	public class HelperPage : System.Web.WebPages.HelperPage
	{
		public static IEntryLinkFactory EntryLinkFactory => (IEntryLinkFactory)DependencyResolver.Current.GetService(typeof(IEntryLinkFactory));

		// Workaround - exposes the MVC HtmlHelper instead of the normal helper
		public static new HtmlHelper Html => ((WebViewPage)WebPageContext.Current.Page).Html;

		public static MarkdownParser MarkdownParser => DependencyResolver.Current.GetService<MarkdownParser>();

		public static IHtmlString ToJS(bool val)
		{
			return new MvcHtmlString(val ? "true" : "false");
		}

		public static IHtmlString ToJS(string str)
		{
			return new MvcHtmlString($@"JSON.parse(""{HttpUtility.JavaScriptStringEncode(JsonHelpers.Serialize(str))}"")");
		}

		public static UrlHelper Url => ((WebViewPage)WebPageContext.Current.Page).Url;

		public static VocaUrlMapper UrlMapper => new VocaUrlMapper();

		public static IUserPermissionContext UserContext => DependencyResolver.Current.GetService<IUserPermissionContext>();

		public static ViewDataDictionary ViewData => ((WebViewPage)WebPageContext.Current.Page).ViewData;
	}
}
