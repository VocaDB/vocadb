using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Security;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Helpers;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Translations;
using VocaDb.Model.Utils;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models {

	public class RegisterModel {

		public RegisterModel() {
			EntryTime = DateTime.Now.Ticks;
		}

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.CreateStrings), ErrorMessageResourceName = "EmailIsRequired")]
		[Display(ResourceType = typeof(ViewRes.User.CreateStrings), Name = "Email")]
		[DataType(DataType.EmailAddress)]
		[StringLength(50)]
		public string Email { get; set; }

		/// <summary>
		/// Time when the form was loaded, to track form fill time
		/// </summary>
		public long EntryTime { get; set; }

		/// <summary>
		/// A decoy field for bots
		/// </summary>
		[StringLength(0)]
		public string Extra { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.CreateStrings), ErrorMessageResourceName = "UsernameIsRequired")]
		[Display(ResourceType = typeof(ViewRes.User.CreateStrings), Name = "Username")]
		[StringLength(100, MinimumLength = 3)]
		[RegularExpression(Model.Domain.Users.User.NameRegex)]
		public string UserName { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.CreateStrings), ErrorMessageResourceName = "PasswordIsRequired")]
		[DataType(DataType.Password)]
		[Display(ResourceType = typeof(ViewRes.User.CreateStrings), Name = "Password")]
		[StringLength(100, MinimumLength = 8)]
		public string Password { get; set; }

	}

	public class LoginModel {

		public LoginModel() {
			this.SecureLogin = true;			
		}

		public LoginModel(string returnUrl, bool returnToMainSite, bool secureLogin)
			: this() {

			this.ReturnUrl = returnUrl;
			this.ReturnToMainSite = returnToMainSite;
			this.SecureLogin = secureLogin;

		}

		[Display(ResourceType = typeof(ViewRes.User.LoginStrings), Name = "KeepMeLoggedIn")]
		public bool KeepLoggedIn { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.LoginStrings), ErrorMessageResourceName = "UsernameIsRequired")]
		[Display(ResourceType = typeof(ViewRes.User.LoginStrings), Name = "Username")]
		[StringLength(100, MinimumLength = 3)]
		public string UserName { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.LoginStrings), ErrorMessageResourceName = "PasswordIsRequired")]
		[DataType(DataType.Password)]
		[Display(ResourceType = typeof(ViewRes.User.LoginStrings), Name = "Password")]
		[StringLength(100)]
		public string Password { get; set; }

		/// <summary>
		/// Whether the user should be returned to the main (HTTP) site.
		/// 
		/// If this is false, the user will be returned to the current site,
		/// which may be either HTTP or HTTPS.
		/// If this is true, the user will always be returned to the main site.
		/// 
		/// This is needed because the HTTP site uses the secure HTTPS site for logging in
		/// and the user should be returned to back to the main HTTP site after login.
		/// </summary>
		public bool ReturnToMainSite { get; set; }

		public string ReturnUrl { get; set; }

		/// <summary>
		/// Whether to use the secure (HTTPS) site for logging in (default is true).
		/// </summary>
		public bool SecureLogin { get; set; }

	}

	[PropertyModelBinder]
	public class MySettingsModel {

		public MySettingsModel() {

			AboutMe = string.Empty;
			AllInterfaceLanguages = InterfaceLanguage.Cultures;
			AllLanguages = EnumVal<ContentLanguagePreference>.Values.ToDictionary(l => l, Translate.ContentLanguagePreferenceName);
			AllStylesheets = AppConfig.SiteSettings.Stylesheets?.Split(',').ToDictionaryWithEmpty(string.Empty, "Default", v => v, v => Path.GetFileNameWithoutExtension(v));
			AllUserKnownLanguages = InterfaceLanguage.UserLanguageCultures;
			AllVideoServices = EnumVal<PVService>.Values;
			Location = string.Empty;
			WebLinks = new List<WebLinkDisplay>();

		}

		public MySettingsModel(UserForMySettingsContract user)
			: this() {
			
			ParamIs.NotNull(() => user);

			AboutMe = user.AboutMe;
			ShowActivity = !user.AnonymousActivity;
			CanChangeName = user.CanChangeName;
			CultureSelection = user.Culture;
			DefaultLanguageSelection = user.DefaultLanguageSelection;
			Email = user.Email;
			EmailOptions = user.EmailOptions;
			EmailVerified = user.EmailVerified;
			HasPassword = user.HasPassword;
			HasTwitterToken = user.HasTwitterToken;
			Id = user.Id;
			InterfaceLanguageSelection = user.Language;
			Location = user.Location;
			KnownLanguages = user.KnownLanguages;
			PreferredVideoService = user.PreferredVideoService;
			PublicAlbumCollection = user.PublicAlbumCollection;
			PublicRatings = user.PublicRatings;
			ShowChatbox = user.ShowChatbox;
			Stylesheet = user.Stylesheet;
			TwitterName = user.TwitterName;
			Username = user.Name;
			UnreadNotificationsToKeep = user.UnreadNotificationsToKeep;
			WebLinks = user.WebLinks.Select(w => new WebLinkDisplay(w)).ToArray();

			AccessKey = user.HashedAccessKey;

		}

		public string AboutMe { get; set; }

		public string AccessKey { get; set; }

		public CultureCollection AllInterfaceLanguages { get; set; }

		public Dictionary<ContentLanguagePreference, string> AllLanguages { get; set; }

		public Dictionary<string, string> AllStylesheets { get; set; }

		public CultureCollection AllUserKnownLanguages { get; set; }

		public PVService[] AllVideoServices { get; set; }

		[Display(Name= "Do not show my name in the recent activity list")]
		public bool ShowActivity { get; set; }

		public bool CanChangeName { get; set; }

		public string CultureSelection { get; set; }

		[Display(Name = "Preferred display language")]
		public ContentLanguagePreference DefaultLanguageSelection { get; set; }

		[Display(Name = "Email options for private messages")]
		public UserEmailOptions EmailOptions { get; set; }

		public bool HasPassword { get; set; }

		public bool HasTwitterToken { get; set; }

		[Display(Name = "Interface language")]
		public string InterfaceLanguageSelection { get; set; }

		[FromJson]
		public UserKnownLanguageContract[] KnownLanguages { get; set; }

		[StringLength(50)]
		public string Location { get; set; }

		[Display(Name = "Preferred streaming service")]
		public PVService PreferredVideoService { get; set; }

		public bool PublicAlbumCollection { get; set; }

		public bool PublicRatings { get; set; }

		public bool ShowChatbox { get; set; }

		[Display(Name = "Email")]
		[DataType(DataType.EmailAddress)]
		[StringLength(50)]
		public string Email { get; set; }

		public bool EmailVerified { get; set; }

		public int Id { get; set; }

		// Note: no validation here because of legacy usernames
		[Display(Name = "Username")]
		[Required]
		public string Username { get; set; }

		[FromJson]
		public IList<WebLinkDisplay> WebLinks { get; set; }

		[Display(Name = "Old password")]
		[DataType(DataType.Password)]
		[StringLength(100)]
		public string OldPass { get; set; }

		[Display(Name = "New password")]
		[DataType(DataType.Password)]
		[System.ComponentModel.DataAnnotations.Compare("NewPassAgain", ErrorMessageResourceType = typeof(ViewRes.User.MySettingsStrings), ErrorMessageResourceName = "PasswordsMustMatch")]
		[StringLength(100)]
		public string NewPass { get; set; }

		[Display(Name = "New password again")]
		[DataType(DataType.Password)]
		[StringLength(100)]
		public string NewPassAgain { get; set; }

		public string Stylesheet { get; set; }

		public string TwitterName { get; set; }

		[Range(1, 100)]
		public int UnreadNotificationsToKeep { get; set; }

		public UpdateUserSettingsContract ToContract() {

			if (WebLinks == null)
				throw new InvalidFormException("Web links list was null");

			return new UpdateUserSettingsContract {
				AboutMe = this.AboutMe ?? string.Empty,
				AnonymousActivity = !this.ShowActivity,
				Culture = this.CultureSelection ?? string.Empty,
				Id = this.Id,
				Name = Username,
				DefaultLanguageSelection = this.DefaultLanguageSelection,
				Email = this.Email ?? string.Empty,
				EmailOptions = this.EmailOptions,
				Language = this.InterfaceLanguageSelection ?? string.Empty,
				Location = this.Location ?? string.Empty,
				KnownLanguages = KnownLanguages ?? new UserKnownLanguageContract[0],
				OldPass = this.OldPass,
				PreferredVideoService = this.PreferredVideoService,
				PublicAlbumCollection = this.PublicAlbumCollection,
				PublicRatings = this.PublicRatings,
				ShowChatbox = this.ShowChatbox,
				Stylesheet = Stylesheet,
				NewPass = this.NewPass,
				UnreadNotificationsToKeep = this.UnreadNotificationsToKeep,
				WebLinks = this.WebLinks.Select(w => w.ToContract()).ToArray(),
			};

		}

	}

	public class UserEdit {

		public UserEdit() {

			var groups = EnumVal<UserGroupId>.Values.Where(g => EntryPermissionManager.CanEditGroupTo(Login.Manager, g)).ToArray();
			EditableGroups = new TranslateableEnum<UserGroupId>(() => global::Resources.UserGroupNames.ResourceManager, groups);
			OwnedArtists = new List<ArtistForUserContract>();
			Permissions = new List<PermissionFlagEntry>();

		}

		public UserEdit(UserWithPermissionsContract contract)
			: this() {

			Active = contract.Active;
			Email = contract.Email;
			GroupId = contract.GroupId;
			Id = contract.Id;
			Name = contract.Name;
			OwnedArtists = contract.OwnedArtistEntries;
			Permissions = PermissionToken.All
				.Select(p => new PermissionFlagEntry(p, contract.AdditionalPermissions.Contains(p), contract.EffectivePermissions.Contains(p))).ToArray();
			Poisoned = contract.Poisoned;
			Supporter = contract.Supporter;

		}

		public bool Active { get; set; }

		public TranslateableEnum<UserGroupId> EditableGroups { get; set; }

		public string Email { get; set; }

		[Display(Name = "User group")]
		public UserGroupId GroupId { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public IList<ArtistForUserContract> OwnedArtists { get; set; }

		public IList<PermissionFlagEntry> Permissions { get; set; }

		public bool Poisoned { get; set; }

		public bool Supporter { get; set; }

		public UserWithPermissionsContract ToContract() {

			return new UserWithPermissionsContract {
				Active = this.Active,
				Email = Email ?? string.Empty,
				GroupId = this.GroupId,
				Id = this.Id,
				Name = this.Name,
				OwnedArtistEntries = OwnedArtists.ToArray(),
				Poisoned = this.Poisoned,
				Supporter = this.Supporter,
				AdditionalPermissions = new HashSet<PermissionToken>(Permissions.Where(p => p.HasFlag).Select(p => PermissionToken.GetById(p.PermissionType.Id)))
			};

		}

	}

	public class PermissionFlagEntry {

		public PermissionFlagEntry() {}

		public PermissionFlagEntry(PermissionToken permissionType, bool hasFlag, bool hasPermission) {
			PermissionType = new PermissionTokenContract(permissionType);
			HasFlag = hasFlag;
			HasPermission = hasPermission;
		}

		public bool HasFlag { get; set; }

		public bool HasPermission { get; set; }

		public PermissionTokenContract PermissionType { get; set; }

	}

}