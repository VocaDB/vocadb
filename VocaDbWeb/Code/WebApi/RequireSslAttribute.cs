using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code.WebApi {

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public class RequireSslAttribute : ActionFilterAttribute {

		public override void OnActionExecuting(HttpActionContext actionContext) {

			#if DEBUG
			return;
			#endif

			if (!WebHelper.IsSSL(actionContext.Request)) {
				// 403.4 - SSL required.
				throw new HttpForbiddenException("This API requires SSL");
			}

		}

	}
}