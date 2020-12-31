using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VocaDb.Web.Code.WebApi
{
	[Obsolete]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public class RequireSslAttribute : ActionFilterAttribute
	{
		// Code from: https://stackoverflow.com/questions/31617345/what-is-the-asp-net-core-mvc-equivalent-to-request-requesturi/40721652#40721652
		private static bool IsSSL(HttpRequest request) => request is not null && new Uri(request.GetDisplayUrl()) is Uri requestUri && requestUri.Scheme == Uri.UriSchemeHttps;

		public override void OnActionExecuting(ActionExecutingContext context)
		{
#if DEBUG
			return;
#endif

			if (!IsSSL(context.HttpContext.Request))
			{
				// 403.4 - SSL required.
				context.Result = new ForbidResult("This API requires SSL");
			}
		}
	}
}