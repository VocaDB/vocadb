using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using NLog;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Helpers;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace VocaDb.Web.Code.WebApi {

	public class RestrictBannedIPAttribute : ActionFilterAttribute {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public override void OnActionExecuting(HttpActionContext actionContext) {

			var ipRules = (IPRuleManager)actionContext.ControllerContext.Configuration.DependencyResolver.GetService(typeof(IPRuleManager));

			var host = WebHelper.GetRealHost(actionContext.Request);

			if (!ipRules.IsAllowed(host)) {

				log.Warn("Restricting banned host '{0}' for '{1}'.", host, actionContext.Request.RequestUri);

				actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);

			}


		}

	}

}