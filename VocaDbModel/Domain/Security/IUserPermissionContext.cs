using System.Diagnostics.CodeAnalysis;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Security
{
	/// <summary>
	/// Information about the currently logged in user.
	/// </summary>
	public interface IUserPermissionContext
	{
#nullable enable
		ContentLanguagePreference LanguagePreference { get; }

		UserSettingLanguagePreference LanguagePreferenceSetting { get; }

		/// <summary>
		/// Whether a user is currently logged in (authenticated).
		/// </summary>
		[MemberNotNullWhen(true, nameof(LoggedUser))]
		bool IsLoggedIn { get; }

		/// <summary>
		/// Currently logged in user. Can be null.
		/// </summary>
		ServerOnlyUserWithPermissionsContract? LoggedUser { get; }

		/// <summary>
		/// Id of the logged in user. If not logged in, 0 (InvalidId) will be returned.
		/// </summary>
		int LoggedUserId { get; }
#nullable disable

		/// <summary>
		/// Name of the currently acting agent. Null if not logged in.
		/// </summary>
		string Name { get; }

#nullable enable
		UserGroupId UserGroupId { get; }

		/// <summary>
		/// Tests whether the user has the specified global permission.
		/// If the user is not logged in they have no permissions.
		/// </summary>
		/// <param name="flag">Permission to be tested.</param>
		/// <returns>True if the user has the permission. For users that are not logged in, this will always be false, except for 'Nothing'.</returns>
		bool HasPermission(PermissionToken flag);

		/// <summary>
		/// Makes sure that the user is logged in (authenticated).
		/// </summary>
		/// <exception cref="NotAllowedException">If the user is not logged in.</exception>
		void VerifyLogin();

		/// <summary>
		/// Makes sure that the user has a specific global permission.
		/// </summary>
		/// <param name="flag">Permission to be tested.</param>
		/// <exception cref="NotAllowedException">If the user does not have the permission.</exception>
		void VerifyPermission(PermissionToken flag);
#nullable disable
	}
}
