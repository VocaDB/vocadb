#nullable disable

using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Users
{
	public class UserGroup
	{
		/// <summary>
		/// User group with no permissions.
		/// </summary>
		public static readonly UserGroup Nothing = new UserGroup(UserGroupId.Nothing);

		private static readonly UserGroup limited = new UserGroup(UserGroupId.Limited, PermissionToken.EditProfile);

		private static readonly UserGroup regular = new UserGroup(UserGroupId.Regular,
			limited,
			PermissionToken.CreateComments,
			PermissionToken.ManageDatabase,
			PermissionToken.EditTags,
			PermissionToken.ReportUser,
			PermissionToken.ManageEventSeries
		);

		private static readonly UserGroup trusted = new UserGroup(UserGroupId.Trusted,
			regular,
			PermissionToken.AddRawFileMedia,
			PermissionToken.ApproveEntries,
			PermissionToken.DeleteEntries,
			PermissionToken.EditFeaturedLists,
			PermissionToken.ManageEntryReports,
			PermissionToken.MergeEntries,
			PermissionToken.RemoveEditPermission,
			PermissionToken.RemoveTagUsages
		);

		private static readonly UserGroup mod = new UserGroup(UserGroupId.Moderator,
			trusted,
			PermissionToken.AccessManageMenu,
			PermissionToken.ApplyAnyTag,
			PermissionToken.BulkDeletePVs,
			PermissionToken.DeleteComments,
			PermissionToken.DesignatedStaff,
			PermissionToken.DisableUsers,
			PermissionToken.EditAllSongLists,
			PermissionToken.LockEntries,
			PermissionToken.ManageIPRules,
			PermissionToken.ManageTagMappings,
			PermissionToken.ManageUserPermissions,
			PermissionToken.MikuDbImport,
			PermissionToken.MoveToTrash,
			PermissionToken.RestoreRevisions,
			PermissionToken.ViewAuditLog,
			PermissionToken.ViewHiddenRatings,
			PermissionToken.ViewHiddenRevisions,
			PermissionToken.UploadMedia
		);

		private static readonly UserGroup admin = new UserGroup(UserGroupId.Admin,
			mod, PermissionToken.Admin);

		private static readonly Dictionary<UserGroupId, UserGroup> groups = new[] {
			limited, regular, trusted, mod, admin
		}.ToDictionary(g => g.Id);

		/// <summary>
		/// Gets a group by Id.
		/// </summary>
		/// <param name="groupId">Group Id.</param>
		/// <returns>User group matching the Id, or <see cref="Nothing"/>, if no matching group is found.</returns>
		public static UserGroup GetGroup(UserGroupId groupId)
		{
			if (!groups.ContainsKey(groupId))
				return Nothing;

			return groups[groupId];
		}

		public static PermissionCollection GetPermissions(UserGroupId groupId)
		{
			if (!groups.ContainsKey(groupId))
				return new PermissionCollection();

			return groups[groupId].Permissions;
		}

		public static UserGroupId[] GroupIds => EnumVal<UserGroupId>.Values;

		public UserGroup(UserGroupId id, UserGroup parent, params PermissionToken[] permissions)
		{
			this.Id = id;
			this.Permissions = parent.Permissions + new PermissionCollection(permissions);
		}

		public UserGroup(UserGroupId id, params PermissionToken[] permissions)
		{
			this.Id = id;
			this.Permissions = new PermissionCollection(permissions);
		}

		public UserGroup(UserGroupId id, PermissionCollection permissions)
		{
			this.Id = id;
			this.Permissions = permissions;
		}

		public UserGroupId Id { get; private set; }

		public PermissionCollection Permissions { get; private set; }
	}

	public enum UserGroupId
	{
		Nothing,

		Limited,

		Regular,

		Trusted,

		Moderator,

		Admin,
	}
}
