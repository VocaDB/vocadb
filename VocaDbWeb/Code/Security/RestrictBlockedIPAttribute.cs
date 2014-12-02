using System.Web.Mvc;
using NLog;

namespace VocaDb.Web.Code.Security {

	/// <summary>
	/// Denies access to Authorized actions for IPs that are restricted.
	/// </summary>
	public class RestrictBlockedIPAttribute : ActionFilterAttribute {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public override void OnActionExecuting(ActionExecutingContext filterContext) {

			if (MvcApplication.IPRules.IsAllowed(filterContext.HttpContext.Request.UserHostAddress))
				return;

			if (filterContext.ActionDescriptor.IsDefined(typeof(AuthorizeAttribute), false)) {
				log.Warn(string.Format("Restricting blocked IP {0}.", filterContext.HttpContext.Request.UserHostAddress));
				filterContext.Result = new HttpUnauthorizedResult();
			}

		}

	}

}