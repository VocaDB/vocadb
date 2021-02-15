// Code from: https://ppolyzos.com/2016/09/09/asp-net-core-render-view-to-string/
// See also: https://stackoverflow.com/questions/59301912/rendering-view-to-string-in-core-3-0-could-not-find-an-irouter-associated-with

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace VocaDb.Web.Helpers
{
	public interface IViewRenderService
	{
		Task<string> RenderToStringAsync(string viewName, object model);
	}

	public class ViewRenderService : IViewRenderService
	{
		private readonly IRazorViewEngine _razorViewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		private readonly IServiceProvider _serviceProvider;
		private readonly IHttpContextAccessor _contextAccessor;

		public ViewRenderService(
			IRazorViewEngine razorViewEngine,
			ITempDataProvider tempDataProvider,
			IServiceProvider serviceProvider,
			IHttpContextAccessor contextAccessor)
		{
			_razorViewEngine = razorViewEngine;
			_tempDataProvider = tempDataProvider;
			_serviceProvider = serviceProvider;
			_contextAccessor = contextAccessor;
		}

		public async Task<string> RenderToStringAsync(string viewName, object model)
		{
			var httpContext = _contextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider };
			var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor());

			using var sw = new StringWriter();
			var viewResult = _razorViewEngine.FindView(actionContext, viewName, isMainPage: false);

			if (viewResult.View == null)
				throw new ArgumentNullException($"{viewName} does not match any available view");

			var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
			{
				Model = model
			};

			var viewContext = new ViewContext(
				actionContext,
				viewResult.View,
				viewDictionary,
				new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
				sw,
				new HtmlHelperOptions()
			);

			await viewResult.View.RenderAsync(viewContext);
			return sw.ToString();
		}
	}
}