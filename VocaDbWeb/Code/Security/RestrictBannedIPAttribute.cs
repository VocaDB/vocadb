using System.Net;
using System.Web.Mvc;
using NLog;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web.Code.Security
{

	/// <summary>
	/// Denies access to an action or controller for IPs that are banned.
	/// </summary>
	public class RestrictBannedIPAttribute : ActionFilterAttribute
	{

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		// Injected by AutoFac.
		public IPRuleManager IPRules { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{

			var host = filterContext.HttpContext.Request.UserHostAddress;

			if (!IPRules.IsAllowed(host))
			{

				log.Warn("Restricting banned host '{0}' for '{1}'.", host, filterContext.HttpContext.Request.Url);

				if (filterContext.HttpContext.Request.IsAjaxRequest())
				{
					filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden);
				}
				else
				{
					filterContext.Result = new RedirectResult("/Error/IPForbidden");
				}

			}

		}

	}
}