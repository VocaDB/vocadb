using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.Security;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Helpers {

	public static class Login {

		public static bool CanAccessManageMenu {
			get {

				return Manager.HasPermission(PermissionToken.AccessManageMenu);

			}
		}

		public static bool CanDeleteEntries {
			get {
				return Manager.HasPermission(PermissionToken.DeleteEntries);
			}
		}

		public static bool CanEditNews {
			get {
				return Manager.HasPermission(PermissionToken.EditNews);
			}
		}

		public static bool CanManageDb {
			get {
				return Manager.HasPermission(PermissionToken.ManageDatabase);
			}
		}

		public static bool CanManageEntryReports {
			get {
				return Manager.HasPermission(PermissionToken.ManageEntryReports);
			}
		}

		public static bool CanManageEventSeries {
			get { return Manager.HasPermission(PermissionToken.ManageEventSeries); }
		}

		public static bool CanManageUsers {
			get {
				return Manager.HasPermission(PermissionToken.ManageUserPermissions);
			}
		}

		public static bool CanMergeEntries {
			get {
				return Manager.HasPermission(PermissionToken.MergeEntries);
			}
		}

		public static bool CanModerateUsers {
			get {
				return Manager.HasPermission(PermissionToken.DisableUsers);
			}
		}

		public static bool CanMoveToTrash {
			get {
				return Manager.HasPermission(PermissionToken.MoveToTrash);
			}
		}

		public static bool CanRevertEntryVersions {
			get {
				return Manager.HasPermission(PermissionToken.RestoreRevisions);
			}
		}

		public static bool IsLoggedIn {
			get {
				return Manager.IsLoggedIn;
			}
		}

		public static LoginManager Manager {
			get {
				return MvcApplication.LoginManager;
			}
		}

		/// <summary>
		/// Currently logged in user. Can be null.
		/// </summary>
		public static UserWithPermissionsContract User {
			get {
				return Manager.LoggedUser;
			}
		}

		public static bool CanDeleteComment(CommentContract comment) {

			ParamIs.NotNull(() => comment);

			return Manager.HasPermission(PermissionToken.DeleteComments)
				|| (comment.Author != null && User != null && comment.Author.Id == User.Id);

		}

		public static bool CanEdit(IEntryWithStatus entry) {

			ParamIs.NotNull(() => entry);

			return EntryPermissionManager.CanEdit(Manager, entry);

		}

	}

}