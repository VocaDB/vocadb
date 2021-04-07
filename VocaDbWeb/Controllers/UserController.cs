#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NLog;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
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
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models;
using VocaDb.Web.Models.User;

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

		private ServerOnlyUserForMySettingsContract GetUserForMySettings()
		{
			return Service.GetUserForMySettings(PermissionContext.LoggedUser.Id);
		}

		private Task SetAuthCookieAsync(string userName, bool createPersistentCookie)
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
			return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
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
			LoginManager loginManager)
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

			return View(new AlbumCollection(Service.GetUser(id, true), routeParams));
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

			return View(user);
		}

		public ActionResult FavoriteSongs(int id = InvalidId, int? page = null, SongVoteRating? rating = null, RatedSongForUserSortRule? sort = null, bool? groupByRating = null)
		{
			if (id == InvalidId)
				return NoId();

			return View(new FavoriteSongs(Service.GetUser(id), rating ?? SongVoteRating.Nothing, sort, groupByRating));
		}

		public ActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> ForgotPassword(ForgotPassword model)
		{
			var captchaResult = await ReCaptcha2.ValidateAsync(new AspNetCoreHttpRequest(Request), AppConfig.ReCAPTCHAKey);
			if (!captchaResult.Success)
				ModelState.AddModelError("CAPTCHA", ViewRes.User.ForgotPasswordStrings.CaptchaIsInvalid);

			if (!ModelState.IsValid)
			{
				return View();
			}

			try
			{
				await Data.RequestPasswordReset(model.Username, model.Email, AppConfig.HostAddress + Url.Action("ResetPassword", "User"));
			}
			catch (UserNotFoundException) { }

			TempData.SetStatusMessage(ViewRes.User.ForgotPasswordStrings.MessageSent);

			return RedirectToAction("Login");
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

			return View(vm);
		}

		//
		// GET: /User/Details/5

		public ActionResult Details(int id = InvalidId)
		{
			if (id == InvalidId)
				return NoId();

			var model = Data.GetUserDetails(id);
			return View(model);
		}

		[Authorize]
		public PartialViewResult OwnedArtistForUserEditRow(int artistId)
		{
			var artist = _artistService.GetArtist(artistId);
			var ownedArtist = new ArtistForUserContract(artist);

			return PartialView(ownedArtist);
		}

		[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3600, VaryByQueryKeys = new[] { "*" })]
		public PartialViewResult PopupContent(int id, string culture = InterfaceLanguage.DefaultCultureCode)
		{
			var user = Service.GetUser(id);
			return PartialView("_UserPopupContent", user);
		}

		public ActionResult Profile(string id, int? artistId = null, bool? childVoicebanks = null)
		{
			var model = Data.GetUserByNameNonSensitive(id);

			if (model == null)
				return NotFound();

			ViewBag.ArtistId = artistId;
			ViewBag.ChildVoicebanks = childVoicebanks;

			return View("Details", model);
		}

		[RestrictBannedIP]
		public new ActionResult Login(string returnUrl = null)
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
				var user = Data.CreateTwitter(model.AccessToken, model.UserName, model.Email ?? string.Empty,
					model.TwitterId, model.TwitterName, Hostname, WebHelper.GetInterfaceCultureName(Request));
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
			return View(new RegisterModel());
		}

		//
		// POST: /User/Create

		[HttpPost]
		public async Task<ActionResult> Create(RegisterModel model)
		{
			string restrictedErr = "Sorry, access from your host is restricted. It is possible this restriction is no longer valid. If you think this is the case, please contact support.";

			if (ModelState[nameof(model.Extra)].Errors.Any())
			{
				s_log.Warn("An attempt was made to fill the bot decoy field from {0} with the value '{1}'.", Hostname, ModelState["Extra"]);
				_ipRuleManager.AddTempBannedIP(Hostname, "Attempt to fill the bot decoy field");
				return View(model);
			}

			if (_config.SiteSettings.SignupsDisabled)
			{
				ModelState.AddModelError(string.Empty, "Signups are disabled");
			}

			var recaptchaResult = await ReCaptcha2.ValidateAsync(new AspNetCoreHttpRequest(Request), AppConfig.ReCAPTCHAKey);
			if (!recaptchaResult.Success)
			{
				ErrorLogger.LogMessage(Request, $"Invalid CAPTCHA (error {recaptchaResult.Error})", LogLevel.Warn);
				_otherService.AuditLog("failed CAPTCHA", Hostname, AuditLogCategory.UserCreateFailCaptcha);
				ModelState.AddModelError("CAPTCHA", ViewRes.User.CreateStrings.CaptchaInvalid);
			}

			if (!ModelState.IsValid)
				return View(model);

			if (!_ipRuleManager.IsAllowed(Hostname))
			{
				s_log.Warn("Restricting blocked IP {0}.", Hostname);
				ModelState.AddModelError("Restricted", restrictedErr);
				return View(model);
			}

			var time = TimeSpan.FromTicks(DateTime.Now.Ticks - model.EntryTime);

			// Attempt to register the user
			try
			{
				var url = VocaUriBuilder.CreateAbsolute(Url.Action("VerifyEmail", "User")).ToString();
				var user = await Data.Create(model.UserName, model.Password, model.Email ?? string.Empty, Hostname,
					Request.Headers[HeaderNames.UserAgent],
					WebHelper.GetInterfaceCultureName(Request),
					time, _ipRuleManager, url);
				await SetAuthCookieAsync(user.Name, createPersistentCookie: false);
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
			catch (TooFastRegistrationException)
			{
				ModelState.AddModelError("Restricted", restrictedErr);
				return View(model);
			}
			catch (RestrictedIPException)
			{
				ModelState.AddModelError("Restricted", restrictedErr);
				return View(model);
			}
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

			var user = Service.GetUserWithPermissions(id);
			return View(new UserEdit(user) { EditableGroups = UserEdit.GetEditableGroups(_loginManager) });
		}

		//
		// POST: /User/Edit/5
		[Authorize]
		[HttpPost]
		public ActionResult Edit(UserEdit model, IEnumerable<PermissionFlagEntry> permissions)
		{
			PermissionContext.VerifyPermission(PermissionToken.ManageUserPermissions);

			model.EditableGroups = UserEdit.GetEditableGroups(_loginManager);
			model.OldName = Service.GetUser(model.Id).Name;

			if (permissions != null)
				model.Permissions = permissions.ToArray();

			if (!ModelState.IsValid)
				return View(model);

			try
			{
				Data.UpdateUser(model.ToContract());
			}
			catch (InvalidEmailFormatException)
			{
				ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.InvalidEmail);
				return View(model);
			}
			catch (UserNameAlreadyExistsException)
			{
				ModelState.AddModelError("Username", "Username is already in use.");
				return View(model);
			}

			return RedirectToAction("Details", new { id = model.Id });
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
			return View(Service.GetUser(id));
		}

		[Authorize]
		public ActionResult Messages(int? messageId, string receiverName)
		{
			var user = PermissionContext.LoggedUser;
			var inbox = UserInboxType.Received;

			if (messageId.HasValue)
			{
				var isNotification = Data.IsNotification(messageId.Value, user);

				if (isNotification)
					inbox = UserInboxType.Notifications;
			}

			var model = new Messages(user, messageId, receiverName, inbox);

			return View(model);
		}

		[Authorize]
		public ActionResult MySettings()
		{
			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			var user = GetUserForMySettings();

			return View(new MySettingsModel(user));
		}

		[HttpPost]
		public async Task<ActionResult> MySettings(MySettingsModel model)
		{
			var user = PermissionContext.LoggedUser;

			if (user == null || user.Id != model.Id)
				return Forbid();

			if (!ModelState.IsValid)
				return View(new MySettingsModel(GetUserForMySettings()));

			ServerOnlyUpdateUserSettingsContract contract;

			try
			{
				contract = model.ToContract();
			}
			catch (InvalidFormException x)
			{
				AddFormSubmissionError(x.Message);
				return View(model);
			}

			ServerOnlyUserWithPermissionsContract newUser;

			try
			{
				newUser = Data.UpdateUserSettings(contract);
				_loginManager.SetLoggedUser(newUser);
				PermissionContext.LanguagePreferenceSetting.Value = model.DefaultLanguageSelection;
			}
			catch (InvalidPasswordException x)
			{
				ModelState.AddModelError("OldPass", x.Message);
				return View(model);
			}
			catch (UserEmailAlreadyExistsException)
			{
				ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.EmailTaken);
				return View(model);
			}
			catch (InvalidEmailFormatException)
			{
				ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.InvalidEmail);
				return View(model);
			}
			catch (InvalidUserNameException)
			{
				ModelState.AddModelError("Username", "Username is invalid. Username may contain alphanumeric characters and underscores.");
				return View(model);
			}
			catch (UserNameAlreadyExistsException)
			{
				ModelState.AddModelError("Username", "Username is already in use.");
				return View(model);
			}
			catch (UserNameTooSoonException)
			{
				ModelState.AddModelError("Username", "Username may only be changed once per year. If necessary, contact a staff member.");
				return View(model);
			}

			// Updating username currently requires signing in again
			if (newUser.Name != user.Name)
			{
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			}

			TempData.SetSuccessMessage(ViewRes.User.MySettingsStrings.SettingsUpdated);

			return RedirectToAction("Profile", new { id = newUser.Name });
		}

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
			return View();
		}

		[HttpPost]
		[Authorize]
		public ActionResult RequestVerification([ModelBinder(BinderType = typeof(JsonModelBinder))] ArtistContract selectedArtist, string message, string linkToProof, bool privateMessage)
		{
			if (selectedArtist == null)
			{
				TempData.SetErrorMessage("Artist must be selected");
				return View("RequestVerification", message);
			}

			if (string.IsNullOrEmpty(linkToProof) && !privateMessage)
			{
				TempData.SetErrorMessage("You must provide a link to proof");
				return View();
			}

			if (string.IsNullOrEmpty(linkToProof) && privateMessage)
			{
				linkToProof = "in a private message";
			}

			var fullMessage = "Proof: " + linkToProof + ", Message: " + message;

			_artistQueries.CreateReport(selectedArtist.Id, ArtistReportType.OwnershipClaim, Hostname, $"Account verification request: {fullMessage}", null);

			TempData.SetSuccessMessage("Request sent");
			return View();
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
