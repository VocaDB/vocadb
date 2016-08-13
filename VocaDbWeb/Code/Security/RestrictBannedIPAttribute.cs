using System.Net;
using System.Web.Mvc;
using NLog;

namespace VocaDb.Web.Code.Security {

	/// <summary>
	/// Denies access to an action or controller for IPs that are banned.
	/// </summary>
	public class RestrictBannedIPAttribute : ActionFilterAttribute {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		// Injected by AutoFac.
		public IPRuleManager IPRules { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext) {

			var host = filterContext.HttpContext.Request.UserHostAddress;

			if (!IPRules.IsAllowed(host)) {

				log.Warn("Restricting banned host '{0}' for '{1}'.", host, filterContext.HttpContext.Request.Url);
				filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, 
					"Sorry, access from your host is restricted. It is possible this restriction is no longer valid. If you think this is the case, please contact support.");

			}

		}

	}
}