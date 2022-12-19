#nullable disable

using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.User;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models;
using VocaDb.Web.Models.User;

namespace VocaDb.Web.Controllers
{
	using NLog;

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

		public static Task SetAuthCookieAsync(HttpContext httpContext, string userName, bool createPersistentCookie)
		{
			// Code from: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, userName),
			};
			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var authProperties = new AuthenticationProperties
			{
				IsPersistent = createPersistentCookie,
			};
			return httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
		}

		private Task SetAuthCookieAsync(string userName, bool createPersistentCookie)
		{
			return SetAuthCookieAsync(HttpContext, userName, createPersistentCookie);
		}

		private async Task<bool> HandleCreateAsync(ServerOnlyUserContract user)
		{
			if (user == null)
			{
				ModelState.AddModelError("UserName", ViewRes.User.CreateStrings.UsernameTaken);
				return false;
			}
			else
			{
				await SetAuthCookieAsync(user.Name, true);
				return true;
			}
		}

		public UserController(
			UserService service,
			UserQueries data,
			ArtistService artistService,
			ArtistQueries artistQueries,
			OtherService otherService,
			IRepository repository,
			UserMessageQueries messageQueries,
			IPRuleManager ipRuleManager,
			VdbConfigManager config,
			MarkdownParser markdownParser,
			ActivityEntryQueries activityEntryQueries,
			LoginManager loginManager
		)
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

		[HttpPost]
		public void AddArtistForUser(int artistId)
		{
			Service.AddArtist(LoggedUserId, artistId);
		}

		public ActionResult AlbumCollection(AlbumCollectionRouteParams routeParams)
		{
			var id = routeParams.id;

			if (id == InvalidId)
				return NoId();

			var model = new AlbumCollection(Service.GetUser(id, true), routeParams);

			PageProperties.Title = "Album collection for " + model.User.Name;

			return View(model);
		}

		public ActionResult ConnectTwitter()
		{
			var props = new AuthenticationProperties
			{
				RedirectUri = "User/ConnectTwitterComplete",
			};
			return Challenge(props, TwitterDefaults.AuthenticationScheme);
		}

		public async Task<ActionResult> ConnectTwitterComplete()
		{
			var cookie = await HttpContext.AuthenticateAsync(AuthenticationConstants.ExternalCookie);

			var accessToken = cookie.Principal.FindFirst(TwitterClaimTypes.AccessToken).Value;

			int.TryParse(cookie.Principal.FindFirst("urn:twitter:userid").Value, out var twitterId);
			var twitterName = cookie.Principal.FindFirst("urn:twitter:screenname").Value;

			if (Service.ConnectTwitter(accessToken, twitterId, twitterName, WebHelper.GetRealHost(Request)))
				TempData.SetStatusMessage("Connected successfully");
			else
				ModelState.AddModelError("Authentication", ViewRes.User.LoginUsingAuthStrings.AuthError);

			await HttpContext.SignOutAsync(AuthenticationConstants.ExternalCookie);

			return RedirectToAction("MySettings");
		}

		public ActionResult EntryEdits(int id = InvalidId, bool onlySubmissions = true)
		{
			if (id == InvalidId)
				return NoId();

			var user = Service.GetUser(id);
			ViewBag.AdditionsOnly = onlySubmissions ? (bool?)true : null;

			PageProperties.Title = "Entry edits - " + user.Name;

			return View("React/Index");
		}

		public ActionResult FavoriteSongs(int id = InvalidId, int? page = null, SongVoteRating? rating = null, RatedSongForUserSortRule? sort = null, bool? groupByRating = null)
		{
			if (id == InvalidId)
				return NoId();

			var model = new FavoriteSongs(Service.GetUser(id), rating ?? SongVoteRating.Nothing, sort, groupByRating);

			PageProperties.Title = "Songs rated by " + model.User.Name;

			return View("React/Index");
		}

		public ActionResult ForgotPassword()
		{
			PageProperties.Title = ViewRes.User.ForgotPasswordStrings.RequestPasswordReset;

			return View("React/Index");
		}

		//
		// GET: /User/

		public ActionResult Index(string filter = null, UserGroupId? groupId = null)
		{
			var vm = new Models.User.Index { Filter = filter, GroupId = groupId };

			if (!string.IsNullOrEmpty(filter))
			{
				var queryParams = new UserQueryParams
				{
					Common = new CommonSearchParams(SearchTextQuery.Create(filter), false, false),
					Paging = new PagingProperties(0, 1, true),
					Group = groupId ?? UserGroupId.Nothing
				};

				var result = Data.GetUsers(queryParams, u => u.Name);

				if (result.TotalCount == 1 && result.Items.Length == 1)
				{
					return RedirectToAction("Profile", new { id = result.Items[0] });
				}
			}

			PageProperties.Title = ViewRes.SharedStrings.Users;

			return View("React/Index");
		}

#nullable enable
		private ActionResult RenderDetails(ServerOnlyUserDetailsContract model)
		{
			PageProperties.Title = model.Name;
			PageProperties.Subtitle = Translate.UserGroups[model.GroupId];
			//PageProperties.CanonicalUrl = VocaUriBuilder.CreateAbsolute(Url.Action("Profile", new { id = model.Name })).ToString();
			PageProperties.Robots = !model.Active ? PagePropertiesData.Robots_Noindex_Follow : string.Empty;

			return View("React/Index");
		}

		//
		// GET: /User/Details/5

		public ActionResult Details(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var model = Data.GetUserDetails(id);

			if (!EntryPermissionManager.CanViewUser(PermissionContext, model))
			{
				return NotFound();
			}

			return RenderDetails(model);
		}

		public ActionResult Profile(string id, int? artistId = null, bool? childVoicebanks = null)
		{
			var model = Data.GetUserByNameNonSensitive(id);

			if (model == null)
				return NotFound();

			if (!EntryPermissionManager.CanViewUser(PermissionContext, model))
			{
				return NotFound();
			}

			ViewBag.ArtistId = artistId;
			ViewBag.ChildVoicebanks = childVoicebanks;

			return RenderDetails(model);
		}
#nullable disable

		[RestrictBannedIP]
		public new ActionResult Login()
		{
			RestoreErrorsFromTempData();

			PageProperties.Title = ViewRes.User.LoginStrings.Login;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View("React/Index");
		}

		[Obsolete("Use /api/users/login instead.")]
		[HttpPost]
		[RestrictBannedIP]
		public new async Task<ActionResult> Login(LoginModel model)
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
					await SetAuthCookieAsync(user.Name, model.KeepLoggedIn);

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

		[RestrictBannedIP]
		public ActionResult LoginTwitter(string returnUrl)
		{
			s_log.Info($"{WebHelper.GetRealHost(Request)} login via Twitter");

			var props = new AuthenticationProperties
			{
				RedirectUri = $"User/LoginTwitterComplete?returnUrl={HttpUtility.UrlEncode(returnUrl)}",
			};
			return Challenge(props, TwitterDefaults.AuthenticationScheme);
		}

		[RestrictBannedIP]
		public async Task<ActionResult> LoginTwitterComplete(string returnUrl)
		{
			var cookie = await HttpContext.AuthenticateAsync(AuthenticationConstants.ExternalCookie);

			var accessToken = cookie.Principal.FindFirst(TwitterClaimTypes.AccessToken).Value;
			var culture = WebHelper.GetInterfaceCultureName(Request);
			var user = Service.CheckTwitterAuthentication(accessToken, Hostname, culture);

			if (user is null)
			{
				int.TryParse(cookie.Principal.FindFirst("urn:twitter:userid").Value, out var twitterId);
				var twitterName = cookie.Principal.FindFirst("urn:twitter:screenname").Value;
				return View(new RegisterOpenAuthModel(accessToken, twitterName, twitterId, twitterName));
			}

			await HandleCreateAsync(user);

			await HttpContext.SignOutAsync(AuthenticationConstants.ExternalCookie);

			var targetUrl = !string.IsNullOrEmpty(returnUrl)
				? VocaUriBuilder.AbsoluteFromUnknown(returnUrl, preserveAbsolute: true)
				: Url.Action("Index", "Home");
			return Redirect(targetUrl);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[RestrictBannedIP]
		public async Task<ActionResult> LoginTwitterComplete(RegisterOpenAuthModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			try
			{
				var user = await Data.CreateTwitter(
					model.AccessToken,
					model.UserName,
					model.Email ?? string.Empty,
					model.TwitterId,
					model.TwitterName,
					Hostname,
					WebHelper.GetInterfaceCultureName(Request));
				await SetAuthCookieAsync(user.Name, false);

				return RedirectToAction("Index", "Home");
			}
			catch (UserNameAlreadyExistsException)
			{
				ModelState.AddModelError("UserName", ViewRes.User.CreateStrings.UsernameTaken);
				return View(model);
			}
			catch (UserEmailAlreadyExistsException)
			{
				ModelState.AddModelError("Email", ViewRes.User.CreateStrings.EmailTaken);
				return View(model);
			}
			catch (InvalidEmailFormatException)
			{
				ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.InvalidEmail);
				return View(model);
			}
		}

		[Obsolete]
		public async Task<ActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}

		[Authorize]
		public ActionResult Clear(int id)
		{
			PermissionContext.VerifyPermission(PermissionToken.DisableUsers);

			Data.ClearRatings(id);
			TempData.SetSuccessMessage("User ratings cleared");
			return RedirectToAction("Details", new { id });
		}

		//
		// GET: /User/Create

		[RestrictBannedIP]
		public ActionResult Create()
		{
			PageProperties.Title = ViewRes.User.CreateStrings.Register;
			PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

			return View("React/Index");
		}

		[HttpPost]
		[Authorize]
		public void DeleteMessage(int messageId)
		{
			_messageQueries.Delete(messageId);
		}

		//
		// GET: /User/Edit/5
		[Authorize]
		public ActionResult Edit(int id)
		{
			PermissionContext.VerifyPermission(PermissionToken.ManageUserPermissions);

			return View("React/Index");
		}

		[ResponseCache(Duration = ClientCacheDurationSec, VaryByQueryKeys = new[] { "*" })]
		public ActionResult Stats_EditsPerDay(int id)
		{
			var points = _activityEntryQueries.GetEditsPerDay(id, null);

			return LowercaseJson(HighchartsHelper.DateLineChartWithAverage("Edits per day", "Edits", "Number of edits", points));
		}

		public ActionResult Stats(int id, string type)
		{
			ViewBag.StatType = type;

			var model = Service.GetUser(id);

			PageProperties.Title = "Stats for " + model.Name;

			return View(model);
		}

#nullable enable
		[Authorize]
		public ActionResult Messages()
		{
			PageProperties.Title = ViewRes.User.MessagesStrings.Messages;

			return View("React/Index");
		}

		[Authorize]
		public ActionResult MySettings()
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return View("React/Index");
		}
#nullable disable

		[HttpPost]
		public void RemoveArtistFromUser(int artistId)
		{
			Service.RemoveArtistFromUser(LoggedUserId, artistId);
		}

		[HttpPost]
		[Authorize]
		public async Task RequestEmailVerification()
		{
			var url = VocaUriBuilder.CreateAbsolute(Url.Action("VerifyEmail", "User"));
			await Data.RequestEmailVerification(LoggedUserId, url.ToString());
		}

		[Authorize]
		public ActionResult VerifyEmail(Guid token)
		{
			try
			{
				var result = Data.VerifyEmail(token);

				if (!result)
				{
					TempData.SetErrorMessage("Request not found or already used.");
					return RedirectToAction("Index", "Home");
				}
				else
				{
					TempData.SetSuccessMessage("Email verified successfully. Thank you.");
					return RedirectToAction("MySettings");
				}
			}
			catch (RequestNotValidException)
			{
				TempData.SetErrorMessage("Verification request is not valid for the logged in user");
				return RedirectToAction("Index", "Home");
			}
		}

		public ActionResult RequestVerification()
		{
			return View("React/Index");
		}

		public ActionResult ResetAccesskey()
		{
			Service.ResetAccessKey();
			TempData.SetStatusMessage("Access key reset");
			return RedirectToAction("MySettings");
		}

		public ActionResult ResetPassword(Guid? id)
		{
			var idVal = id ?? Guid.Empty;
			var model = new ResetPassword();

			if (!Data.CheckPasswordResetRequest(idVal))
			{
				ModelState.AddModelError("", "Request ID is invalid. It might have been used already.");
			}
			else
			{
				model.RequestId = idVal;
			}

			return View(model);
		}

		[HttpPost]
		public async Task<ActionResult> ResetPassword(ResetPassword model)
		{
			if (!Data.CheckPasswordResetRequest(model.RequestId))
			{
				ModelState.AddModelError("", "Request ID is invalid. It might have been used already.");
			}

			if (!ModelState.IsValid)
			{
				return View(new ResetPassword());
			}

			var user = Data.ResetPassword(model.RequestId, model.NewPass);
			await SetAuthCookieAsync(user.Name, createPersistentCookie: false);

			TempData.SetStatusMessage("Password reset successfully!");

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public void UpdateAlbumForUser(int albumid, PurchaseStatus collectionStatus, MediaType mediaType, int rating)
		{
			Data.UpdateAlbumForUser(LoggedUserId, albumid, collectionStatus, mediaType, rating);
		}

		[HttpPost]
		public void UpdateArtistSubscription(int artistId, bool? emailNotifications, bool? siteNotifications)
		{
			Data.UpdateArtistSubscriptionForCurrentUser(artistId, emailNotifications, siteNotifications);
		}

		[Authorize]
		public ActionResult Disable(int id)
		{
			Data.DisableUser(id);

			return RedirectToAction("Details", new { id });
		}

		[Authorize]
		public ActionResult DisconnectTwitter()
		{
			Data.DisconnectTwitter();

			TempData.SetSuccessMessage("Twitter login disconnected");

			return RedirectToAction("MySettings");
		}
	}
}
