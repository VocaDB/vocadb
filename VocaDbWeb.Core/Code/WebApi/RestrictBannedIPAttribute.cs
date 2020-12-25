using System;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code.WebApi
{
	public class RestrictBannedIPAttribute : ActionFilterAttribute
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var ipRules = context.HttpContext.RequestServices.GetRequiredService<IPRuleManager>();

			var host = WebHelper.GetRealHost(context.HttpContext.Request);

			if (!ipRules.IsAllowed(host))
			{
				// Code from: https://stackoverflow.com/questions/31617345/what-is-the-asp-net-core-mvc-equivalent-to-request-requesturi/40721652#40721652
				s_log.Warn($"Restricting banned host '{host}' for '{new Uri(context.HttpContext.Request.GetDisplayUrl())}'.");

				context.Result = new ForbidResult();
			}
		}
	}
}