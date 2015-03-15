using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Security {

	public interface IUserPermissionContext {

		ContentLanguagePreference LanguagePreference { get; }

		UserSettingLanguagePreference LanguagePreferenceSetting { get; }

		bool IsLoggedIn { get; }

		/// <summary>
		/// Currently logged in user. Can be null.
		/// </summary>
		UserWithPermissionsContract LoggedUser { get; }

		/// <summary>
		/// Id of the logged in user. If not logged in, 0 (InvalidId) will be returned.
		/// </summary>
		int LoggedUserId { get; }

		/// <summary>
		/// Name of the currently acting agent. Null if not logged in.
		/// </summary>
		string Name { get; }

		UserSettingShowChatbox ShowChatbox { get; }

		UserGroupId UserGroupId { get; }

		/// <summary>
		/// Tests whether the user has the specified permission flag.
		/// If the user is not logged in they have no permissions.
		/// </summary>
		/// <param name="flag">Permission token to be tested.</param>
		/// <returns>True if the user has the permission. For users that are not logged in, this will always be false, except for 'Nothing'.</returns>
		bool HasPermission(PermissionToken flag);

		void VerifyLogin();

		void VerifyPermission(PermissionToken flag);

	}

}
