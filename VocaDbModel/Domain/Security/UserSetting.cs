#nullable disable

using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Model.Domain.Security;

using VocaDb.Model.Helpers;

/// <summary>
/// Common interface for user settings that can be changed individually.
/// These settings can be persisted for the user account if the user is logged in, or in a cookie if the user has no account.
/// </summary>
public interface IUserSetting
{
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

public abstract class UserSetting<T> : IUserSetting
{
	private string GetCookieValue(IHttpRequest request, string cookieName)
	{
		if (_context == null)
			return null;

		var cookie = request.Cookies.GetValueOrDefault(cookieName);

		if (cookie == null || string.IsNullOrEmpty(cookie.Value))
			return null;
		else
			return cookie.Value;
	}

	private void SetCookie(IHttpContext context, string cookieName, string value, TimeSpan expires)
	{
		if (context != null)
		{
			context.Response.AddCookie(cookieName, value, DateTime.Now + expires);
		}
	}

	private bool TryGetCookieValue(IHttpRequest request, string cookieName, ref T value, Func<string, T> valueGetter)
	{
		var cookieValue = GetCookieValue(request, cookieName);

		if (cookieValue == null)
			return false;

		value = valueGetter(cookieValue);

		return true;
	}

	delegate bool ValueGetterDelegate(string str, out T val);

	private static bool TryGetFromQueryString(IHttpRequest request, string paramName, ref T value, ValueGetterDelegate valueGetter)
	{
		if (request == null || string.IsNullOrEmpty(paramName) || string.IsNullOrEmpty(request.QueryString[paramName]))
			return false;

		return valueGetter(request.QueryString[paramName], out value);
	}

	private readonly IHttpContext _context;
	private readonly IUserPermissionContext _permissionContext;

	protected virtual TimeSpan CookieExpires => TimeSpan.FromHours(24);

	protected virtual string CookieName => $"UserSettings.{SettingName}";

	protected virtual T Default => default(T);

	private bool IsRequestValueOverridden => _context != null && _context.Items.Contains(RequestItemName);

	private T ParseValue(string str) => TryParseValue(str, out T val) ? val : Default;

	private IHttpRequest Request => _context != null ? _context.Request : null;

	protected virtual string RequestParamName => null;

	private string RequestItemName => $"UserSettings.{SettingName}";

	private T RequestValue
	{
		get
		{
			if (_context == null)
				throw new InvalidOperationException("HttpContext is not initialized");

			return (T)_context.Items[RequestItemName];
		}
		set
		{
			if (_context == null)
				throw new InvalidOperationException("HttpContext is not initialized");

			_context.Items[RequestItemName] = value;
		}
	}

	protected abstract string SettingName { get; }

	protected abstract T GetPersistedValue(ServerOnlyUserWithPermissionsContract permissionContext);

	public void OverrideRequestValue(T val)
	{
		RequestValue = val;
	}

	public void ParseFromValue(string val)
	{
		Value = ParseValue(val);
	}

	protected abstract void SetPersistedValue(User user, T val);

	protected abstract void SetPersistedValue(ServerOnlyUserWithPermissionsContract user, T val);

#nullable enable
	public override string ToString()
	{
		return $"User setting {SettingName}: {Value}";
	}
#nullable disable

	protected abstract bool TryParseValue(string str, out T val);

	protected UserSetting(IHttpContext context, IUserPermissionContext permissionContext)
	{
		_context = context;
		_permissionContext = permissionContext;
	}

	public void UpdateUser(User user)
	{
		SetPersistedValue(user, Value);
	}

	/// <summary>
	/// Gets or sets the current value. 
	/// This will be persisted for the request, but not yet into the database (use <see cref="UpdateUser"/> to persist DB).
	/// </summary>
	public T Value
	{
		get
		{
			if (IsRequestValueOverridden)
				return RequestValue;

			var val = Default;
			if (TryGetFromQueryString(Request, RequestParamName, ref val, TryParseValue))
				return val;

			if (_permissionContext.IsLoggedIn)
				return GetPersistedValue(_permissionContext.LoggedUser);

			if (TryGetCookieValue(Request, CookieName, ref val, ParseValue))
				return val;

			return val;
		}
		set
		{
			if (IsRequestValueOverridden)
				OverrideRequestValue(value);

			if (_permissionContext.IsLoggedIn)
				SetPersistedValue(_permissionContext.LoggedUser, value);

			SetCookie(_context, CookieName, value.ToString(), CookieExpires);
		}
	}
}

public class UserSettingLanguagePreference : UserSetting<ContentLanguagePreference>
{
	public UserSettingLanguagePreference(IHttpContext context, IUserPermissionContext permissionContext)
		: base(context, permissionContext) { }

	protected override ContentLanguagePreference Default => ContentLanguagePreference.Default;

	protected override string RequestParamName => "lang";

	protected override string SettingName => "LanguagePreference";

	protected override ContentLanguagePreference GetPersistedValue(ServerOnlyUserWithPermissionsContract user)
	{
		return user.DefaultLanguageSelection;
	}

	protected override void SetPersistedValue(User user, ContentLanguagePreference val)
	{
		user.DefaultLanguageSelection = val;
	}

	protected override void SetPersistedValue(ServerOnlyUserWithPermissionsContract user, ContentLanguagePreference val)
	{
		user.DefaultLanguageSelection = val;
	}

	protected override bool TryParseValue(string str, out ContentLanguagePreference val)
	{
		return Enum.TryParse(str, out val);
	}
}
