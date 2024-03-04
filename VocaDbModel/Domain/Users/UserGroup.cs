using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Users;

public enum UserGroupId
{
	Nothing,
	Limited,
	Regular,
	Trusted,
	Moderator,
	Admin,
}

public class UserGroup
{
	/// <summary>
	/// User group with no permissions.
	/// </summary>
	public static readonly UserGroup Nothing = new(id: UserGroupId.Nothing);

	private static readonly UserGroup s_limited = new(id: UserGroupId.Limited, PermissionToken.EditProfile);

	public static readonly UserGroup s_regular = new(
		id: UserGroupId.Regular,
		parent: s_limited,
		PermissionToken.CreateComments,
		PermissionToken.ManageDatabase,
		PermissionToken.EditTags,
		PermissionToken.ReportUser,
		PermissionToken.ManageEventSeries,
		PermissionToken.ViewLyrics,
		PermissionToken.ViewCoverArtImages
	);

	private static readonly UserGroup s_trusted = new(
		id: UserGroupId.Trusted,
		parent: s_regular,
		PermissionToken.AddRawFileMedia,
		PermissionToken.ApproveEntries,
		PermissionToken.DeleteEntries,
		PermissionToken.EditFeaturedLists,
		PermissionToken.ManageEntryReports,
		PermissionToken.MergeEntries,
		PermissionToken.RemoveEditPermission,
		PermissionToken.RemoveTagUsages
	);

	private static readonly UserGroup s_mod = new(
		id: UserGroupId.Moderator,
		parent: s_trusted,
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
		PermissionToken.UploadMedia,
		PermissionToken.ViewOldUsernames,
		PermissionToken.ViewDisabledUsers
	);

	private static readonly UserGroup s_admin = new(
		id: UserGroupId.Admin,
		parent: s_mod,
		PermissionToken.Admin,
		PermissionToken.ManageWebhooks,
		PermissionToken.CreateDatabaseDump
	);

	private static readonly Dictionary<UserGroupId, UserGroup> s_groups = new[]
	{
		s_limited,
		s_regular,
		s_trusted,
		s_mod,
		s_admin,
	}.ToDictionary(g => g.Id);

	/// <summary>
	/// Gets a group by Id.
	/// </summary>
	/// <param name="groupId">Group Id.</param>
	/// <returns>User group matching the Id, or <see cref="Nothing"/>, if no matching group is found.</returns>
	public static UserGroup GetGroup(UserGroupId groupId)
	{
		if (!s_groups.ContainsKey(groupId))
			return Nothing;

		return s_groups[groupId];
	}

	public static PermissionCollection GetPermissions(UserGroupId groupId)
	{
		if (!s_groups.ContainsKey(groupId))
			return new PermissionCollection();

		return s_groups[groupId].Permissions;
	}

	public static UserGroupId[] GroupIds => EnumVal<UserGroupId>.Values;

	public UserGroup(UserGroupId id, UserGroup parent, params PermissionToken[] permissions)
	{
		Id = id;
		Permissions = parent.Permissions + new PermissionCollection(permissions);
	}

	public UserGroup(UserGroupId id, params PermissionToken[] permissions)
	{
		Id = id;
		Permissions = new PermissionCollection(permissions);
	}

	public UserGroup(UserGroupId id, PermissionCollection permissions)
	{
		Id = id;
		Permissions = permissions;
	}

	public UserGroupId Id { get; private set; }

	public PermissionCollection Permissions { get; private set; }
}
