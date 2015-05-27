using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.Messages;
using Microsoft.Web.Helpers;
using NLog;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Models;
using VocaDb.Web.Models.User;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers
{
    public class UserController : ControllerBase
    {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private readonly ArtistQueries artistQueries;
		private readonly ArtistService artistService;
		private readonly VdbConfigManager config;
		private UserQueries Data { get; set; }
		private readonly IPRuleManager ipRuleManager;
		private readonly MarkdownParser markdownParser;
		private readonly UserMessageQueries messageQueries;
		private readonly OtherService otherService;
	    private UserService Service { get; set; }

		private UserForMySettingsContract GetUserForMySettings() {

			return Service.GetUserForMySettings(PermissionContext.LoggedUser.Id);

		}

		private bool HandleCreate(UserContract user) {

			if (user == null) {
				ModelState.AddModelError("UserName", ViewRes.User.CreateStrings.UsernameTaken);
				return false;
			} else {
				FormsAuthentication.SetAuthCookie(user.Name, false);
				return true;
			}

		}

		public UserController(UserService service, UserQueries data, ArtistService artistService, ArtistQueries artistQueries, OtherService otherService, 
			UserMessageQueries messageQueries, IPRuleManager ipRuleManager, VdbConfigManager config, MarkdownParser markdownParser) {

			Service = service;
			Data = data;
			this.artistQueries = artistQueries;
			this.artistService = artistService;
			this.otherService = otherService;
			this.messageQueries = messageQueries;
			this.ipRuleManager = ipRuleManager;
			this.config = config;
			this.markdownParser = markdownParser;

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void AddArtistForUser(int artistId) {

			Service.AddArtist(LoggedUserId, artistId);

		}

		public ActionResult AlbumCollection(AlbumCollectionRouteParams routeParams) {

			var id = routeParams.id;

			if (id == invalidId)
				return NoId();

			return View(new AlbumCollection(Service.GetUser(id, true), routeParams));

		}

		public ActionResult ConnectTwitter() {

			// Make sure session ID is initialized
			// ReSharper disable UnusedVariable
			var sessionId = Session.SessionID;
			// ReSharper restore UnusedVariable

			var twitterSignIn = new TwitterConsumer().TwitterSignIn;

			var uri = new Uri(new Uri(AppConfig.HostAddress), Url.Action("ConnectTwitterComplete"));

			UserAuthorizationRequest request;
			try {
				request = twitterSignIn.PrepareRequestUserAuthorization(uri, null, null);
			} catch (ProtocolException x) {
				log.Fatal("Exception while attempting to sent Twitter request", x);	
				TempData.SetErrorMessage("There was an error while connecting to Twitter - please try again later.");
				return RedirectToAction("MySettings", "User");
			}

			var response = twitterSignIn.Channel.PrepareResponse(request);

			response.Send();
			Response.End();
			return new EmptyResult();

		}

		public ActionResult ConnectTwitterComplete() {

			// Denied authorization
			var param = Request.QueryString["denied"];

			if (!string.IsNullOrEmpty(param)) {
				TempData.SetStatusMessage(ViewRes.User.LoginUsingAuthStrings.SignInCancelled);
				return RedirectToAction("MySettings");
			}

			var response = new TwitterConsumer().ProcessUserAuthorization(Hostname);

			if (response == null) {
				TempData.SetStatusMessage(ViewRes.User.LoginUsingAuthStrings.AuthError);
				return RedirectToAction("MySettings");
			}

			int twitterId;
			int.TryParse(response.ExtraData["user_id"], out twitterId);
			var twitterName = response.ExtraData["screen_name"];

			if (Service.ConnectTwitter(response.AccessToken, twitterId, twitterName, WebHelper.GetRealHost(Request))) {
				TempData.SetStatusMessage("Connected successfully");
			} else {
				ModelState.AddModelError("Authentication", ViewRes.User.LoginUsingAuthStrings.AuthError);
			}

			return RedirectToAction("MySettings");

		}

		public ActionResult EntryEdits(int id = invalidId, bool onlySubmissions = true) {

			if (id == invalidId)
				return NoId();

			var user = Service.GetUser(id);
			ViewBag.EditEvent = (onlySubmissions ? (int?)EntryEditEvent.Created : null);

			return View(user);

		}

		public ActionResult FavoriteSongs(int id = invalidId, int? page = null, SongVoteRating? rating = null, SongSortRule? sort = null, bool? groupByRating = null) {

			if (id == invalidId)
				return NoId();

			return View(new FavoriteSongs(Service.GetUser(id), rating ?? SongVoteRating.Nothing, sort, groupByRating));

		}

		public ActionResult ForgotPassword() {

			return View();

		}

		[HttpPost]
		public ActionResult ForgotPassword(ForgotPassword model) {

			if (!ReCaptcha.Validate(ConfigurationManager.AppSettings["ReCAPTCHAKey"]))
				ModelState.AddModelError("CAPTCHA", ViewRes.User.ForgotPasswordStrings.CaptchaIsInvalid);

			if (!ModelState.IsValid) {
				return View();
			}

			try {
				Data.RequestPasswordReset(model.Username, model.Email, AppConfig.HostAddress + Url.Action("ResetPassword", "User"));
				TempData.SetStatusMessage(ViewRes.User.ForgotPasswordStrings.MessageSent);
				return RedirectToAction("Login");
			} catch (UserNotFoundException) {
				ModelState.AddModelError("Username", ViewRes.User.ForgotPasswordStrings.UsernameDoesNotMatch);
				return View();
			}

		}

			//
        // GET: /User/

        public ActionResult Index(string filter = null) {

			if (!string.IsNullOrEmpty(filter)) {

				var result = Data.GetUsers(SearchTextQuery.Create(filter), UserGroupId.Nothing, false, false, UserSortRule.Name, new PagingProperties(0, 1, true), u => u.Name);

				if (result.TotalCount == 1 && result.Items.Length == 1) {
					return RedirectToAction("Profile", new { id = result.Items[0] });
				}

				ViewBag.Filter = filter;
				
			}

			return View();

        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id = invalidId) {

	        if (id == invalidId)
		        return NoId();

			var model = Data.GetUserDetails(id);
			return View(model);

		}

		[Authorize]
		public PartialViewResult OwnedArtistForUserEditRow(int artistId) {

			var artist = artistService.GetArtist(artistId);
			var ownedArtist = new ArtistForUserContract(artist);

			return PartialView(ownedArtist);

		}

		public new ActionResult Profile(string id) {

			var model = Data.GetUserByNameNonSensitive(id);

			if (model == null)
				return HttpNotFound();

			return View("Details", model);

		}

		[RestrictBannedIP]
		public ActionResult Login(string returnUrl, bool secureLogin = true)
        {

			RestoreErrorsFromTempData();

            return View(new LoginModel(returnUrl, !WebHelper.IsSSL(Request), secureLogin));
        }

		[RestrictBannedIP]
		public PartialViewResult LoginForm(string returnUrl, bool secureLogin = true) {

		   return PartialView("Login", new LoginModel(returnUrl, !WebHelper.IsSSL(Request), secureLogin));

		}

        //
        // POST: /Session/Create

        [HttpPost]
		[RestrictBannedIP]
		public ActionResult Login(LoginModel model)
        {

			if (ModelState.IsValid) {

				var host = WebHelper.GetRealHost(Request);
				var culture = WebHelper.GetInterfaceCultureName(Request);
				var result = Data.CheckAuthentication(model.UserName, model.Password, host, culture, true);

				if (!result.IsOk) {

					ModelState.AddModelError("", ViewRes.User.LoginStrings.WrongPassword);
					
					if (result.Error == LoginError.AccountPoisoned)
						ipRuleManager.TempBannedIPs.Add(host);

				} else {

					var user = result.User;

					TempData.SetSuccessMessage(string.Format(ViewRes.User.LoginStrings.Welcome, user.Name));
					FormsAuthentication.SetAuthCookie(user.Name, model.KeepLoggedIn);

					var redirectUrl = FormsAuthentication.GetRedirectUrl(model.UserName, true);
					string targetUrl;

					// TODO: should not allow redirection to URLs outside the site
					if (!string.IsNullOrEmpty(model.ReturnUrl)) {
						targetUrl = model.ReturnUrl;				
					} else if (!string.IsNullOrEmpty(redirectUrl))
						targetUrl = redirectUrl;
					else
						targetUrl = Url.Action("Index", "Home");

					if (model.ReturnToMainSite)
						targetUrl = VocaUriBuilder.AbsoluteFromUnknown(targetUrl, preserveAbsolute: true, ssl: false);

					return Redirect(targetUrl);

				}

			}

			if (model.ReturnToMainSite) {
				SaveErrorsToTempData();
				return Redirect(VocaUriBuilder.Absolute(Url.Action("Login", new { model.ReturnUrl, model.SecureLogin }), false));				
			}

        	return View(model);

		}

		public ActionResult LoginTwitter(string returnUrl) {
			
			// Make sure session ID is initialized
// ReSharper disable UnusedVariable
			var sessionId = Session.SessionID;
// ReSharper restore UnusedVariable

			var twitterSignIn = new TwitterConsumer().TwitterSignIn;

			var targetUrl = Url.Action("LoginTwitterComplete", new { returnUrl });
			var uri = new Uri(new Uri(AppConfig.HostAddress), targetUrl);

			UserAuthorizationRequest request;

			try {
				request = twitterSignIn.PrepareRequestUserAuthorization(uri, null, null);
			} catch (ProtocolException x) {
				
				log.Error("Exception while attempting to send Twitter request", x);	
				TempData.SetErrorMessage("There was an error while connecting to Twitter - please try again later.");

				return RedirectToAction("Login");

			}

			var response = twitterSignIn.Channel.PrepareResponse(request);

			response.Send();
			Response.End();
			
			return new EmptyResult();

		}

		public ActionResult LoginTwitterComplete(string returnUrl) {

			// Denied authorization
			var param = Request.QueryString["denied"];

			if (!string.IsNullOrEmpty(param)) {
				TempData.SetStatusMessage(ViewRes.User.LoginUsingAuthStrings.SignInCancelled);
				return View("Login", new LoginModel(string.Empty, !WebHelper.IsSSL(Request), true));
			}

			var response = new TwitterConsumer().ProcessUserAuthorization(Hostname);

			if (response == null) {
				ModelState.AddModelError("Authentication", ViewRes.User.LoginUsingAuthStrings.AuthError);
				return View("Login", new LoginModel(string.Empty, !WebHelper.IsSSL(Request), true));
			}

			var culture = WebHelper.GetInterfaceCultureName(Request);
			var user = Service.CheckTwitterAuthentication(response.AccessToken, Hostname, culture);

			if (user == null) {
				int twitterId;
				int.TryParse(response.ExtraData["user_id"], out twitterId);
				var twitterName = response.ExtraData["screen_name"];
				return View(new RegisterOpenAuthModel(response.AccessToken, twitterName, twitterId, twitterName));
			}

			HandleCreate(user);

			string targetUrl;

			if (!string.IsNullOrEmpty(returnUrl))
				targetUrl = VocaUriBuilder.AbsoluteFromUnknown(returnUrl, preserveAbsolute: true, ssl: false);
			else
				targetUrl = Url.Action("Index", "Home");

			return Redirect(targetUrl);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LoginTwitterComplete(RegisterOpenAuthModel model) {

			if (!ModelState.IsValid)
				return View(model);

			try {

				var user = Data.CreateTwitter(model.AccessToken, model.UserName, model.Email ?? string.Empty,
					model.TwitterId, model.TwitterName, Hostname, WebHelper.GetInterfaceCultureName(Request));
				FormsAuthentication.SetAuthCookie(user.Name, false);

				return RedirectToAction("Index", "Home");

			} catch (UserNameAlreadyExistsException) {

				ModelState.AddModelError("UserName", ViewRes.User.CreateStrings.UsernameTaken);
				return View(model);

			} catch (UserEmailAlreadyExistsException) {

				ModelState.AddModelError("Email", ViewRes.User.CreateStrings.EmailTaken);
				return View(model);

			} catch (InvalidEmailFormatException) {

				ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.InvalidEmail);
				return View(model);

			}

		}

		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			return RedirectToAction("Index", "Home");
		}

		[Authorize]
		public ActionResult Clear(int id) {
			
			PermissionContext.VerifyPermission(PermissionToken.DisableUsers);

			Data.ClearRatings(id);
			TempData.SetSuccessMessage("User ratings cleared");
			return RedirectToAction("Details", new { id });

		}

		[HttpPost]
		public ActionResult ComposeMessage(ComposeMessage model) {

			if (!ModelState.IsValid) {
				SaveErrorsToTempData();
				return RedirectToAction("Messages");
			}

			var contract = model.ToContract(LoggedUserId);
			var mySettingsUrl = VocaUriBuilder.CreateAbsolute(Url.Action("MySettings", "User")).ToString();
			var messagesUrl = VocaUriBuilder.CreateAbsolute(Url.Action("Messages", "User")).ToString();

			try {
				Service.SendMessage(contract, mySettingsUrl, messagesUrl);
			} catch (UserNotFoundException x) {
				ModelState.AddModelError("ReceiverName", x.Message);
				SaveErrorsToTempData();
				return RedirectToAction("Messages");
			}

			TempData.SetStatusMessage(ViewRes.User.MessagesStrings.MessageSent);

			return RedirectToAction("Messages");

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
        public ActionResult Create(RegisterModel model) {

			string restrictedErr = "Sorry, access from your host is restricted. It is possible this restriction is no longer valid. If you think this is the case, please contact support.";

			if (!ModelState.IsValidField("Extra")) {
				log.Warn(string.Format("An attempt was made to fill the bot decoy field from {0}.", Hostname));
				ipRuleManager.TempBannedIPs.Add(Hostname);
				return View(model);				
			}

			if (config.SiteSettings.SignupsDisabled) {
				ModelState.AddModelError(string.Empty, "Signups are disabled");
			}

			if (!ReCaptcha.Validate(ConfigurationManager.AppSettings["ReCAPTCHAKey"])) {

				var captchaResponse = Request.Params["recaptcha_response_field"] ?? string.Empty;
				ErrorLogger.LogMessage(Request, string.Format("Invalid CAPTCHA (response was {0})", captchaResponse), LogLevel.Warn);
				otherService.AuditLog("failed CAPTCHA", Hostname, AuditLogCategory.UserCreateFailCaptcha);
				ModelState.AddModelError("CAPTCHA", ViewRes.User.CreateStrings.CaptchaInvalid);

			}

			if (!ModelState.IsValid)
				return View(model);

			if (!ipRuleManager.IsAllowed(Hostname)) {
				ModelState.AddModelError("Restricted", restrictedErr);
				return View(model);
			}

			var time = TimeSpan.FromTicks(DateTime.Now.Ticks - model.EntryTime);

	        // Attempt to register the user
	        try {

				var url = VocaUriBuilder.CreateAbsolute(Url.Action("VerifyEmail", "User")).ToString();
				var user = Data.Create(model.UserName, model.Password, model.Email ?? string.Empty, Hostname, 
					WebHelper.GetInterfaceCultureName(Request),
					time, ipRuleManager.TempBannedIPs, url);
				FormsAuthentication.SetAuthCookie(user.Name, false);
		        return RedirectToAction("Index", "Home");

	        } catch (UserNameAlreadyExistsException) {

		        ModelState.AddModelError("UserName", ViewRes.User.CreateStrings.UsernameTaken);
		        return View(model);

	        } catch (UserEmailAlreadyExistsException) {

				ModelState.AddModelError("Email", ViewRes.User.CreateStrings.EmailTaken);
				return View(model);
      
	        } catch (InvalidEmailFormatException) {

				ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.InvalidEmail);
				return View(model);

	        } catch (TooFastRegistrationException) {

				ModelState.AddModelError("Restricted", restrictedErr);
				return View(model);

	        }

        }

		[HttpPost]
		[Authorize]
		public void DeleteMessage(int messageId) {
			messageQueries.Delete(messageId);
		}

        //
        // GET: /User/Edit/5
		[Authorize]
		public ActionResult Edit(int id)
        {

			PermissionContext.VerifyPermission(PermissionToken.ManageUserPermissions);

        	var user = Service.GetUserWithPermissions(id);
            return View(new UserEdit(user));

        }

        //
        // POST: /User/Edit/5
		[Authorize]
        [HttpPost]
		public ActionResult Edit(UserEdit model, IEnumerable<PermissionFlagEntry> permissions) {

			PermissionContext.VerifyPermission(PermissionToken.ManageUserPermissions);

			if (permissions != null)
				model.Permissions = permissions.ToArray();

			Service.UpdateUser(model.ToContract());

        	return RedirectToAction("Details", new {id = model.Id});

        }

		[OutputCache(Duration = statsCacheDurationSec)]
		public ActionResult SongsPerGenre(int id = invalidId) {
			
			if (id == invalidId)
				return NoId();

			return LowercaseJson(Data.GetRatingsByGenre(id));

		}

		[Authorize]
		public ActionResult MessageBody(int messageId = invalidId) {

			if (messageId == invalidId)
				return NoId();

			var msg = messageQueries.Get(messageId, null);
			var body = markdownParser.GetHtml(msg.Body);
			return Content(body);

		}

		[Authorize]
		public ActionResult Messages(int? messageId, string receiverName) {

			var user = PermissionContext.LoggedUser;
			RestoreErrorsFromTempData();
			var model = new Messages(user, messageId, receiverName);

			return View(model);

		}

		[Authorize]
		public ActionResult MessagesJson(int maxCount = 100, int start = 0, bool unread = false, int iconSize = 20) {

			var messages = messageQueries.GetList(LoggedUserId, new PagingProperties(start, maxCount, false), unread, new GravatarUserIconFactory(iconSize));
			return LowercaseJson(messages);

		}

		[Authorize]
		public ActionResult MySettings() {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);			

			var user = GetUserForMySettings();

			return View(new MySettingsModel(user));

		}

		[HttpPost]
		public ActionResult MySettings(MySettingsModel model) {

			var user = PermissionContext.LoggedUser;

			if (user == null || user.Id != model.Id)
				return new HttpStatusCodeResult(403);

			if (!ModelState.IsValid)
				return View(new MySettingsModel(GetUserForMySettings()));

			try {
				var newUser = Data.UpdateUserSettings(model.ToContract());
				LoginManager.SetLoggedUser(newUser);
				PermissionContext.LanguagePreferenceSetting.Value = model.DefaultLanguageSelection;
			} catch (InvalidPasswordException x) {
				ModelState.AddModelError("OldPass", x.Message);
				return View(model);
			} catch (UserEmailAlreadyExistsException) {
				ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.EmailTaken);
				return View(model);
			} catch (InvalidEmailFormatException) {
				ModelState.AddModelError("Email", ViewRes.User.MySettingsStrings.InvalidEmail);
				return View(model);
			}

			TempData.SetSuccessMessage(ViewRes.User.MySettingsStrings.SettingsUpdated);

			return RedirectToAction("Profile", new { id = user.Name });

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void RemoveArtistFromUser(int artistId) {

			Service.RemoveArtistFromUser(LoggedUserId, artistId);

		}

		[HttpPost]
		[Authorize]
		public void RequestEmailVerification() {
			
			var url = VocaUriBuilder.CreateAbsolute(Url.Action("VerifyEmail", "User"));
			Data.RequestEmailVerification(LoggedUserId, url.ToString());

		}

		[Authorize]
		public ActionResult VerifyEmail(Guid token) {

			try {
				var result = Data.VerifyEmail(token);

				if (!result) {
					TempData.SetErrorMessage("Request not found or already used.");
					return RedirectToAction("Index", "Home");
				} else {
					TempData.SetSuccessMessage("Email verified successfully. Thank you.");
					return RedirectToAction("MySettings");
				}

			} catch (RequestNotValidException) {
				TempData.SetErrorMessage("Verification request is not valid for the logged in user");
				return RedirectToAction("Index", "Home");
			}

		}

		public ActionResult RequestVerification() {

			return View();

		}

		[HttpPost]
		[Authorize]
		public ActionResult RequestVerification([FromJson] ArtistContract selectedArtist, string message) {

			if (selectedArtist == null) {
				TempData.SetErrorMessage("Artist must be selected");
				return View("RequestVerification", null, message);
			}

			if (string.IsNullOrEmpty(message)) {
				TempData.SetErrorMessage("You must enter some message");
				return View();
			}

			artistQueries.CreateReport(selectedArtist.Id, ArtistReportType.OwnershipClaim, Hostname, string.Format("Account verification request: {0}", message), null);

			TempData.SetSuccessMessage("Request sent");
			return View();

		}

		public ActionResult ResetAccesskey() {

			Service.ResetAccessKey();
			TempData.SetStatusMessage("Access key reset");
			return RedirectToAction("MySettings");

		}

		public ActionResult ResetPassword(Guid? id) {

			var idVal = id ?? Guid.Empty;
			var model = new ResetPassword();

			if (!Data.CheckPasswordResetRequest(idVal)) {
				ModelState.AddModelError("", "Request ID is invalid. It might have been used already.");
			} else {
				model.RequestId = idVal;
			}

			return View(model);

		}

		[HttpPost]
		public ActionResult ResetPassword(ResetPassword model) {

			if (!Data.CheckPasswordResetRequest(model.RequestId)) {
				ModelState.AddModelError("", "Request ID is invalid. It might have been used already.");
			}

			if (!ModelState.IsValid) {
				return View(new ResetPassword());
			}

			var user = Data.ResetPassword(model.RequestId, model.NewPass);
			FormsAuthentication.SetAuthCookie(user.Name, false);

			TempData.SetStatusMessage("Password reset successfully!");

			return RedirectToAction("Index", "Home");

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void UpdateAlbumForUser(int albumid, PurchaseStatus collectionStatus, MediaType mediaType, int rating) {

			Data.UpdateAlbumForUser(LoggedUserId, albumid, collectionStatus, mediaType, rating);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void UpdateArtistSubscription(int artistId, bool? emailNotifications, bool? siteNotifications) {
			
			Data.UpdateArtistSubscriptionForCurrentUser(artistId, emailNotifications, siteNotifications);

		}

		[AcceptVerbs(HttpVerbs.Post)]
		public void AddSongToFavorites(int songId, SongVoteRating rating = SongVoteRating.Favorite) {

			Service.UpdateSongRating(LoggedUserId, songId, rating);

		}

		[Authorize]
		public ActionResult Disable(int id) {

			Data.DisableUser(id);

			return RedirectToAction("Details", new { id });

		}

		[Authorize]
		public ActionResult DisconnectTwitter() {
			
			Data.DisconnectTwitter();

			TempData.SetSuccessMessage("Twitter login disconnected");

			return RedirectToAction("MySettings");

		}

    }
}
