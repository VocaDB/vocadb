using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NLog;
using VocaDb.Model;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Web.Code;

namespace VocaDb.Web.Middleware
{
	public class VocaDbPrincipalMiddleware
	{
		private readonly RequestDelegate _next;

		public VocaDbPrincipalMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		private static bool IsAjaxRequest(HttpRequest request)
		{
			ParamIs.NotNull(() => request);

			return request.Headers["X-Requested-With"] == "XMLHttpRequest";
		}

		public Task Invoke(HttpContext context, LoginManager loginManager, UserService userService)
		{
			try
			{
				// Get user roles from cookie and assign correct principal
				if (context.User.Identity is not null && context.User.Identity.IsAuthenticated)
				{
					if (context.User is not VocaDbPrincipal)
					{
						var id = context.User.Identity;
						var user = userService.GetUserByName(id.Name, IsAjaxRequest(context.Request));
						if (user != null)
							loginManager.SetLoggedUser(user);
					}
				}

				loginManager.InitLanguage();
			}
			catch (Exception x)
			{
				// TODO: this should be processed using normal exception handling.
				ErrorLogger.LogException(context.Request, x, LogLevel.Fatal);
			}

			return _next(context);
		}
	}

	public static class VocaDbPrincipalMiddlewareExtensions
	{
		public static IApplicationBuilder UseVocaDbPrincipal(this IApplicationBuilder builder) => builder.UseMiddleware<VocaDbPrincipalMiddleware>();
	}
}
