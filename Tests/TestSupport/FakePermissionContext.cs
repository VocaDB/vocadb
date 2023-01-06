#nullable disable

using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.TestSupport;

public class FakePermissionContext : IUserPermissionContext
{
	public FakePermissionContext() { }

	public FakePermissionContext(ServerOnlyUserWithPermissionsContract loggedUser)
	{
		LoggedUser = loggedUser;
	}

	public FakePermissionContext(User user)
		: this(new ServerOnlyUserWithPermissionsContract(user, ContentLanguagePreference.Default)) { }

	public bool HasPermission(PermissionToken token)
	{
		if (token == PermissionToken.Nothing)
			return true;

		if (!IsLoggedIn || !LoggedUser.Active)
			return false;

		return (LoggedUser.EffectivePermissions.Contains(token));
	}

	public bool IsLoggedIn => LoggedUser != null;

	public ContentLanguagePreference LanguagePreference { get; set; }

	public UserSettingLanguagePreference LanguagePreferenceSetting => throw new System.NotImplementedException();

	public ServerOnlyUserWithPermissionsContract LoggedUser { get; set; }

	public int LoggedUserId => LoggedUser?.Id ?? 0;

	public string Name { get; set; }

	public UserGroupId UserGroupId => LoggedUser != null ? LoggedUser.GroupId : UserGroupId.Nothing;

	public void GrantPermission(PermissionToken permissionToken)
	{
		LoggedUser?.EffectivePermissions.Add(permissionToken);
	}

	public void LogOff()
	{
		LoggedUser = null;
	}

	/// <summary>
	/// Updates status, including permissions, of the currently logged in user.
	/// </summary>
	/// <typeparam name="T">Repository type.</typeparam>
	/// <param name="repository">Repository. Cannot be null.</param>
	public void RefreshLoggedUser<T>(IRepository<T> repository) where T : class, IDatabaseObject
	{
		LoggedUser = repository.HandleQuery(ctx => new ServerOnlyUserWithPermissionsContract(ctx.OfType<User>().Load(LoggedUserId), ContentLanguagePreference.Default));
	}

	public void SetLoggedUser(User user)
	{
		LoggedUser = new ServerOnlyUserWithPermissionsContract(user, ContentLanguagePreference.Default);
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
			throw new NotAllowedException();
		}
	}
}
