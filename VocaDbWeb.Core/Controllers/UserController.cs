#nullable disable

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NLog;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models;

namespace VocaDb.Web.Controllers
{
	public class UserController : ControllerBase
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		private const int ClientCacheDurationSec = 86400;

		private readonly ActivityEntryQueries _activityEntryQueries;
		private readonly ArtistQueries _artistQueries;
		private readonly ArtistService _artistService;
		private readonly VdbConfigManager _config;
		private UserQueries Data { get; set; }
		private readonly IPRuleManager _ipRuleManager;
		private readonly LoginManager _loginManager;
		private readonly MarkdownParser _markdownParser;
		private readonly UserMessageQueries _messageQueries;
		private readonly OtherService _otherService;
		private readonly IRepository _repository;
		private UserService Service { get; set; }

		public UserController(UserService service, UserQueries data, ArtistService artistService, ArtistQueries artistQueries, OtherService otherService,
			IRepository repository,
			UserMessageQueries messageQueries, IPRuleManager ipRuleManager, VdbConfigManager config, MarkdownParser markdownParser, ActivityEntryQueries activityEntryQueries, LoginManager loginManager)
		{
			Service = service;
			Data = data;
			_activityEntryQueries = activityEntryQueries;
			_artistQueries = artistQueries;
			_artistService = artistService;
			_repository = repository;
			_otherService = otherService;
			_messageQueries = messageQueries;
			_ipRuleManager = ipRuleManager;
			_config = config;
			_markdownParser = markdownParser;
			_loginManager = loginManager;
		}

		[RestrictBannedIP]
		public ActionResult Login(string returnUrl = null)
		{
			RestoreErrorsFromTempData();

			return View(new LoginModel(returnUrl, false));
		}

		[RestrictBannedIP]
		public PartialViewResult LoginForm(string returnUrl)
		{
			return PartialView("Login", new LoginModel(returnUrl, false));
		}

		[HttpPost]
		[RestrictBannedIP]
		public async Task<ActionResult> Login(LoginModel model)
		{
			if (ModelState.IsValid)
			{
				var host = WebHelper.GetRealHost(Request);
				var culture = WebHelper.GetInterfaceCultureName(Request);
				var result = Data.CheckAuthentication(model.UserName, model.Password, host, culture, true);

				if (!result.IsOk)
				{
					ModelState.AddModelError("", ViewRes.User.LoginStrings.WrongPassword);

					if (result.Error == LoginError.AccountPoisoned)
						_ipRuleManager.AddTempBannedIP(host, "Account poisoned");
				}
				else
				{
					var user = result.User;

					TempData.SetSuccessMessage(string.Format(ViewRes.User.LoginStrings.Welcome, user.Name));
					// Code from: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, user.Name),
					};
					var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var authProperties = new AuthenticationProperties
					{
						IsPersistent = model.KeepLoggedIn,
					};
					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

					string redirectUrl = null;
					// TODO: implement
					/*try
					{
						redirectUrl = FormsAuthentication.GetRedirectUrl(model.UserName, true);
					}
					catch (HttpException x)
					{
						s_log.Warn(x, "Unable to get redirect URL");
					}*/

					string targetUrl;

					// TODO: should not allow redirection to URLs outside the site
					if (!string.IsNullOrEmpty(model.ReturnUrl))
						targetUrl = model.ReturnUrl;
					else if (!string.IsNullOrEmpty(redirectUrl))
						targetUrl = redirectUrl;
					else
						targetUrl = Url.Action("Index", "Home");

					if (model.ReturnToMainSite)
						targetUrl = VocaUriBuilder.AbsoluteFromUnknown(targetUrl, preserveAbsolute: true);

					return Redirect(targetUrl);
				}
			}

			if (model.ReturnToMainSite)
			{
				SaveErrorsToTempData();
				return Redirect(VocaUriBuilder.Absolute(Url.Action("Login", new { model.ReturnUrl })));
			}

			return View(model);
		}

		public async Task<ActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}
	}
}
