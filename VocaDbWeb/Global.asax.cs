using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using NLog;
using NHibernate;
using VocaDb.Model;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Security;
using VocaDb.Web.App_Start;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Security;

namespace VocaDb.Web {

	public class MvcApplication : HttpApplication {

		private static readonly HostCollection bannedIPs = new HostCollection();
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static UserService UserService {
			get {
				return DependencyResolver.Current.GetService<UserService>();				
			}
		}

		public static bool IsAjaxRequest(HttpRequest request) {

			ParamIs.NotNull(() => request);

			return (request["X-Requested-With"] == "XMLHttpRequest" || request.Headers["X-Requested-With"] == "XMLHttpRequest");

		}

		public static HostCollection BannedIPs {
			get { return bannedIPs; }
		}

		public static LoginManager LoginManager {
			get {
				return new LoginManager();
			}
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e) {

			try {

				// Get user roles from cookie and assign correct principal
				if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated) {
					if (HttpContext.Current.User.Identity is FormsIdentity && !(HttpContext.Current.User is VocaDbPrincipal)) {
						var id = (FormsIdentity)HttpContext.Current.User.Identity;
						var user = UserService.GetUserByName(id.Name, IsAjaxRequest(Request));
						if (user != null)
							LoginManager.SetLoggedUser(user);
					}
				}

				LoginManager.InitLanguage();

			} catch (HttpRequestValidationException x) {
				ErrorLogger.LogMessage(Request, x.Message, LogLevel.Warn);
			} catch (Exception x) {
				// TODO: this should be processed using normal exception handling.
				ErrorLogger.LogException(Request, x, LogLevel.Fatal);
			}

		}

		private void HandleHttpError(int code, string description = null, string msg = null) {

			// Log error here to get request info.
			ErrorLogger.LogHttpError(Request, code, msg);

			Server.ClearError();
			Response.StatusCode = code;

			if (!string.IsNullOrEmpty(description))
				Response.StatusDescription = description;

			Response.RedirectToRoute("Default",
				new { controller = "Error", action = "Index", code, redirect = true });

		}

		protected void Application_Error(object sender, EventArgs e) {

			var ex = HttpContext.Current.Server.GetLastError();

			if (ex == null)
				return;

			if (ex is OperationCanceledException) {
				// This is a bug in Web API 5.1 (http://aspnetwebstack.codeplex.com/workitem/1797) and can be ignored.
				Server.ClearError();
				return;
			}

			// NHibernate missing entity exceptions. Usually caused by an invalid or deleted Id. This error has been logged already.
			if (ex is ObjectNotFoundException || ex is EntityNotFoundException) {
				HandleHttpError(ErrorLogger.Code_NotFound, "Entity not found");
				return;
			}

			// Insufficient privileges. This error has been logged already.
			if (ex is NotAllowedException) {
				HandleHttpError(ErrorLogger.Code_Forbidden, ex.Message);
				return;
			}

			// Not found (usually by the controller). We get these a lot.
			var httpException = ex as HttpException;
			var code = (httpException != null ? httpException.GetHttpCode() : 0);
			if (code == ErrorLogger.Code_NotFound || code == ErrorLogger.Code_BadRequest) {
				// Getting a lot of these >_>
				if (ex.Message != "A potentially dangerous Request.Path value was detected from the client (?).") {
					HandleHttpError(code, null, ex.Message);
				} else {
					Server.ClearError();
					Response.StatusCode = code;		
				}
				return;					
			}

			// Generic NHibernate exception. This error has been logged already.
			if (!(ex is HibernateException))
				ErrorLogger.LogException(Request, ex);

			// Redirect user to generic error page in release mode or display detailed message in debug mode
#if !DEBUG
			Server.ClearError();
			Response.RedirectToRoute("Default", new { controller = "Error", action = "Index", redirect = true });
#endif
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			//filters.Add(new HandleErrorAttribute { ExceptionType = typeof(ObjectNotFoundException), View = "NotFound" });
			//filters.Add(new HandleErrorAttribute());
		}

		protected void Application_Start() {

			log.Info("Web application starting.");

			AreaRegistration.RegisterAllAreas();

			GlobalConfiguration.Configure(WebApiConfig.Configure);
			ComponentConfig.RegisterComponent();
			RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			log.Debug("Web application started successfully.");

		}
	}
}