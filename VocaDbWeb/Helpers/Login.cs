using System;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.Security;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Helpers
{
	public class Login
	{
		private readonly LoginManager _manager;

		public Login(LoginManager manager)
		{
			_manager = manager;
		}

		public bool CanAccessManageMenu => Manager.HasPermission(PermissionToken.AccessManageMenu);

		public bool CanDeleteEntries => Manager.HasPermission(PermissionToken.DeleteEntries);

		public bool CanManageDb => Manager.HasPermission(PermissionToken.ManageDatabase);

		public bool CanEditTags => Manager.HasPermission(PermissionToken.EditTags);

		public bool CanManageEntryReports => Manager.HasPermission(PermissionToken.ManageEntryReports);

		[Obsolete]
		public bool CanManageEventSeries => Manager.HasPermission(PermissionToken.ManageEventSeries);

		public bool CanManageUsers => Manager.HasPermission(PermissionToken.ManageUserPermissions);

		public bool CanMergeEntries => Manager.HasPermission(PermissionToken.MergeEntries);

		public bool CanModerateUsers => Manager.HasPermission(PermissionToken.DisableUsers);

		public bool CanMoveToTrash => Manager.HasPermission(PermissionToken.MoveToTrash);

		public bool CanRevertEntryVersions => Manager.HasPermission(PermissionToken.RestoreRevisions);

		public bool CanViewHiddenRevisions => Manager.HasPermission(PermissionToken.ViewHiddenRevisions);

		public bool IsLoggedIn => Manager.IsLoggedIn;

		public LoginManager Manager => _manager;

		/// <summary>
		/// Currently logged in user. Can be null.
		/// </summary>
		public ServerOnlyUserWithPermissionsContract User => Manager.LoggedUser;

		public bool CanDeleteComment(CommentForApiContract comment)
		{
			ParamIs.NotNull(() => comment);

			return Manager.HasPermission(PermissionToken.DeleteComments)
				|| (comment.Author != null && User != null && comment.Author.Id == User.Id);
		}

		public bool CanEdit(IEntryWithStatus entry)
		{
			ParamIs.NotNull(() => entry);

			return EntryPermissionManager.CanEdit(Manager, entry);
		}
	}
}
