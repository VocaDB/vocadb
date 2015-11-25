using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using VocaDb.Web.Controllers;

namespace VocaDb.Web.Helpers {

	public static class RazorHelper {

		public static string RenderPartialViewToString<T>(string viewName, T model, string controllerName, HttpRequestMessage request) {

			using (var writer = new StringWriter()) {

				var routeData = new RouteData();
				routeData.Values.Add("controller", controllerName);
				var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, request.RequestUri.ToString(), null), new HttpResponse(null))), routeData, new HelpController(null));

				var razorViewEngine = new RazorViewEngine();
				var razorViewResult = razorViewEngine.FindPartialView(fakeControllerContext, viewName, false);

				var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(model), new TempDataDictionary(), writer);
				razorViewResult.View.Render(viewContext, writer);
				return writer.ToString();
			}

		}
	}
}