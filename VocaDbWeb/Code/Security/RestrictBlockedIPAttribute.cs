#nullable disable

using System.Web.Mvc;
using NLog;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web.Code.Security
{
	/// <summary>
	/// Denies access to Authorized actions for IPs that are restricted.
	/// This filter is applied globally for all actions marked with the <see cref="AuthorizeAttribute"/>.
	/// </summary>
	public class RestrictBlockedIPAttribute : ActionFilterAttribute
	{
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		private readonly IPRuleManager _ipRuleManager;

		public RestrictBlockedIPAttribute(IPRuleManager ipRuleManager)
		{
			_ipRuleManager = ipRuleManager;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var host = filterContext.HttpContext.Request.UserHostAddress;

			if (_ipRuleManager.IsAllowed(host))
				return;

			if (filterContext.ActionDescriptor.IsDefined(typeof(AuthorizeAttribute), false))
			{
				_log.Warn("Restricting blocked IP {0}.", host);
				filterContext.Result = new HttpUnauthorizedResult();
			}
		}
	}
}