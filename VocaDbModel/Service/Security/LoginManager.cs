#nullable disable

using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Principal;
using NLog;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Web;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.Security;

/// <summary>
/// Manages login and culture related properties per-request.
/// </summary>
public class LoginManager : IUserPermissionContext
{
#nullable enable
	public LoginManager(IHttpContext context)
	{
		_context = context;
	}

	private readonly IHttpContext _context;
	public const int InvalidId = 0;
	public const string LangParamName = "lang";

	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

	private ServerOnlyUserWithPermissionsContract? _user;

	private void SetCultureSafe(string? name, bool culture, bool uiCulture)
	{
		if (string.IsNullOrEmpty(name))
			return;

		try
		{
			var c = CultureInfo.GetCultureInfo(name);

			if (culture)
				Thread.CurrentThread.CurrentCulture = c;

			if (uiCulture)
				Thread.CurrentThread.CurrentUICulture = c;
		}
		catch (ArgumentException x)
		{
			s_log.Warn(x, "Unable to set culture");
		}
	}

	public static string GetHashedAccessKey(string? key)
	{
		var salt = ConfigurationManager.AppSettings["AccessKeySalt"] ?? string.Empty;

		return CryptoHelper.HashSHA1(key + salt);
	}

	public void SetLoggedUser(ServerOnlyUserWithPermissionsContract user)
	{
		ParamIs.NotNull(() => user);

		if (_context.User.Identity is null || !_context.User.Identity.IsAuthenticated)
			throw new InvalidOperationException("Must be authenticated");

		_context.User = new VocaDbPrincipal(_context.User.Identity, user);
	}

	protected IPrincipal User => _context.User;

	public bool HasPermission(PermissionToken token)
	{
		if (token == PermissionToken.Nothing)
			return true;

		if (!IsLoggedIn || !LoggedUser.Active)
			return false;

		if (token == PermissionToken.ManageDatabase && LockdownEnabled)
			return false;

		return (LoggedUser.EffectivePermissions.Contains(token));
	}

	[MemberNotNullWhen(true, nameof(LoggedUser))]
	public bool IsLoggedIn => (_context != null && User != null && User.Identity is not null && User.Identity.IsAuthenticated && User is VocaDbPrincipal);

	public ContentLanguagePreference LanguagePreference => LanguagePreferenceSetting.Value;

	public UserSettingLanguagePreference LanguagePreferenceSetting => new UserSettingLanguagePreference(_context, this);

	public bool LockdownEnabled => !string.IsNullOrEmpty(AppConfig.LockdownMessage);

	/// <summary>
	/// Currently logged in user. Can be null.
	/// </summary>
	public ServerOnlyUserWithPermissionsContract? LoggedUser
	{
		get
		{
			if (_user != null)
				return _user;

			_user = (IsLoggedIn ? ((VocaDbPrincipal)User).User : null);
			return _user;
		}
	}

	/// <summary>
	/// Logged user Id or InvalidId if no user is logged in.
	/// </summary>
	public int LoggedUserId => LoggedUser != null ? LoggedUser.Id : InvalidId;
#nullable disable

	public string Name => User.Identity.Name;

#nullable enable
	public UserGroupId UserGroupId
	{
		get
		{
			if (LoggedUser == null)
				return UserGroupId.Nothing;

			return LoggedUser.GroupId;
		}
	}

	public void InitLanguage()
	{
		if (_context != null && !string.IsNullOrEmpty(_context.Request.Params["culture"]))
		{
			var cName = _context.Request.Params["culture"];
			SetCultureSafe(cName, true, true);
		}
		else if (IsLoggedIn)
		{
			SetCultureSafe(LoggedUser.Culture, true, false);
			SetCultureSafe(LoggedUser.Language, false, true);
		}
	}

	public void VerifyLogin()
	{
		if (!IsLoggedIn)
			throw new NotAllowedException("Must be logged in.");
	}

	public void VerifyPermission(PermissionToken flag)
	{
		if (!HasPermission(flag))
		{
			s_log.Warn("User '{0}' does not have the requested permission '{1}'", Name, flag);
			throw new NotAllowedException();
		}
	}
#nullable disable
}
