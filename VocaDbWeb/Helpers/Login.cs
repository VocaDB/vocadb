#nullable disable

using System;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.Security;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Helpers
{
	public static class Login
	{
		public static bool CanAccessManageMenu => Manager.HasPermission(PermissionToken.AccessManageMenu);

		public static bool CanDeleteEntries => Manager.HasPermission(PermissionToken.DeleteEntries);

		public static bool CanManageDb => Manager.HasPermission(PermissionToken.ManageDatabase);

		public static bool CanEditTags => Manager.HasPermission(PermissionToken.EditTags);

		public static bool CanManageEntryReports => Manager.HasPermission(PermissionToken.ManageEntryReports);

		[Obsolete]
		public static bool CanManageEventSeries => Manager.HasPermission(PermissionToken.ManageEventSeries);

		public static bool CanManageUsers => Manager.HasPermission(PermissionToken.ManageUserPermissions);

		public static bool CanMergeEntries => Manager.HasPermission(PermissionToken.MergeEntries);

		public static bool CanModerateUsers => Manager.HasPermission(PermissionToken.DisableUsers);

		public static bool CanMoveToTrash => Manager.HasPermission(PermissionToken.MoveToTrash);

		public static bool CanRevertEntryVersions => Manager.HasPermission(PermissionToken.RestoreRevisions);

		public static bool CanViewHiddenRevisions => Manager.HasPermission(PermissionToken.ViewHiddenRevisions);

		public static bool IsLoggedIn => Manager.IsLoggedIn;

		public static LoginManager Manager => MvcApplication.LoginManager;

		/// <summary>
		/// Currently logged in user. Can be null.
		/// </summary>
		public static UserWithPermissionsContract User => Manager.LoggedUser;

		public static bool CanDeleteComment(CommentForApiContract comment)
		{
			ParamIs.NotNull(() => comment);

			return Manager.HasPermission(PermissionToken.DeleteComments)
				|| (comment.Author != null && User != null && comment.Author.Id == User.Id);
		}

		public static bool CanEdit(IEntryWithStatus entry)
		{
			ParamIs.NotNull(() => entry);

			return EntryPermissionManager.CanEdit(Manager, entry);
		}
	}
}
