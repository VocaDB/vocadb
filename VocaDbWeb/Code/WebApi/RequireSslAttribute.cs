#nullable disable

using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using VocaDb.Web.Code.Exceptions;

namespace VocaDb.Web.Code.WebApi
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public class RequireSslAttribute : ActionFilterAttribute
	{
		private static bool IsSSL(HttpRequestMessage request)
		{
			return request != null && request.RequestUri != null && request.RequestUri.Scheme == Uri.UriSchemeHttps;
		}

		public override void OnActionExecuting(HttpActionContext actionContext)
		{
#if DEBUG
			return;
#endif

			if (!IsSSL(actionContext.Request))
			{
				// 403.4 - SSL required.
				throw new HttpForbiddenException("This API requires SSL");
			}
		}
	}
}