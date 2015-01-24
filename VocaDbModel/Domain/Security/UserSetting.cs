using System;
using System.Web;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Security {

	/// <summary>
	/// Common interface for user settings that can be changed individually.
	/// These settings can be persisted for the user account if the user is logged in, or in a cookie if the user has no account.
	/// </summary>
	public interface IUserSetting {

		/// <summary>
		/// Parses setting value from a string.
		/// </summary>
		/// <param name="val">String to be parsed, for example 'false'.</param>
		void ParseFromValue(string val);

		/// <summary>
		/// Persists the setting for a logged in user.
		/// </summary>
		/// <param name="user">User to be updated. Cannot be null.</param>
		void UpdateUser(User user);

	}

	public abstract class UserSetting<T> : IUserSetting {

		private static string GetCookieValue(HttpRequest request, string cookieName) {
			
			if (HttpContext.Current == null)
				return null;

			var cookie = request.Cookies.Get(cookieName);

			if (cookie == null || string.IsNullOrEmpty(cookie.Value))
				return null;
			else
				return cookie.Value;

		}

		private static void SetCookie(HttpContext context, string cookieName, string value, TimeSpan expires) {
			
			if (context != null) {
				var cookie = new HttpCookie(cookieName, value) { Expires = DateTime.Now + expires };
				context.Response.Cookies.Add(cookie);
			}

		}

		private static bool TryGetCookieValue<T>(HttpRequest request, string cookieName, ref T value, Func<string, T> valueGetter) {
			
			var cookieValue = GetCookieValue(request, cookieName);

			if (cookieValue == null)
				return false;

			value = valueGetter(cookieValue);

			return true;

		}

		private readonly HttpContext context;
		private readonly IUserPermissionContext permissionContext;

		protected virtual TimeSpan CookieExpires {
			get {
				return TimeSpan.FromHours(24);
			}
		}

		protected abstract string CookieName { get; }

		protected virtual T Default {
			get {
				return default(T);
			}
		}

		protected abstract T ParseValue(string str);

		protected abstract T GetPersistedValue(UserWithPermissionsContract permissionContext);

		public void ParseFromValue(string val) {
			Value = ParseValue(val);
		}

		protected abstract void SetPersistedValue(User user, T val);

		protected abstract void SetPersistedValue(UserWithPermissionsContract user, T val);

		protected UserSetting(HttpContext context, IUserPermissionContext permissionContext) {
			this.context = context;
			this.permissionContext = permissionContext;
		}

		public void UpdateUser(User user) {
			SetPersistedValue(user, Value);
		}

		/// <summary>
		/// Gets or sets the current value. 
		/// This will be persisted for the request, but not yet into the database (use <see cref="UpdateUser"/> to persist DB).
		/// </summary>
		public T Value {
			get {

				if (permissionContext.IsLoggedIn)
					return GetPersistedValue(permissionContext.LoggedUser);

				var val = Default;
				if (TryGetCookieValue(context.Request, CookieName, ref val, ParseValue))
					return val;
				
				return val;
					
			}
			set {

				if (permissionContext.IsLoggedIn)
					SetPersistedValue(permissionContext.LoggedUser, value);

				SetCookie(context, CookieName, value.ToString(), CookieExpires);					

			}
		}

	}

	public class UserSettingShowChatbox : UserSetting<bool> {

		public UserSettingShowChatbox(HttpContext context, IUserPermissionContext permissionContext) : base(context, permissionContext) {}

		protected override string CookieName {
			get { return "showChatbox"; }
		}

		protected override bool Default {
			get { return true; }
		}

		protected override bool ParseValue(string str) {
			return bool.Parse(str);
		}

		protected override bool GetPersistedValue(UserWithPermissionsContract user) {
			return user.ShowChatbox;
		}

		protected override void SetPersistedValue(User user, bool val) {
			user.Options.ShowChatbox = val;
		}

		protected override void SetPersistedValue(UserWithPermissionsContract user, bool val) {
			user.ShowChatbox = val;
		}

	}

}
