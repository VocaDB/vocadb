using System;
using System.Collections.Immutable;
using System.Linq;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Security {

	using StatusSet = ImmutableSortedSet<EntryStatus>;

	public static class EntryPermissionManager {

		private static StatusSet Set(params EntryStatus[] vals) {
			return ImmutableSortedSet.CreateRange(vals);
		}

		private static readonly ImmutableSortedSet<EntryStatus> allPermissions = Set(EnumVal<EntryStatus>.Values);

		// Entry statuses allowed for normal users
		private static readonly ImmutableSortedSet<EntryStatus> normalStatusPermissions = Set(EntryStatus.Draft, EntryStatus.Finished);

		// Entry statuses allowed for trusted users
		private static readonly ImmutableSortedSet<EntryStatus> trustedStatusPermissions = Set(EntryStatus.Draft, EntryStatus.Finished, EntryStatus.Approved);

		private static bool IsVerifiedFor(IUserPermissionContext permissionContext, IEntryBase entry) {
			
			return permissionContext.LoggedUser.VerifiedArtist && permissionContext.LoggedUser.OwnedArtistEntries.Any(a => a.Artist.Id == entry.Id);

		}

		/// <summary>
		/// Gets a list of entry statuses that the user can edit or set.
		/// This means, the user is allowed to edit entries with any of these statuses, 
		/// and the user is able to change the entry status to any of these.
		/// </summary>
		/// <remarks>
		/// Most of the time the allowed entry statuses are global, but associating a user account with an artist entry
		/// gives special entry-specific permissions for the user editing that entry.
		/// </remarks>
		/// <param name="permissionContext">User permission context identifying the user's global permissions.</param>
		/// <param name="entry">Entry to be checked. Can be null. If null, only global permissions will be checked.</param>
		/// <returns>A list of permissions that can be set by the user.</returns>
		public static StatusSet AllowedEntryStatuses(IUserPermissionContext permissionContext, IEntryBase entry = null) {

			// Check for basic edit permissions, without these the user is limited or disabled
			if (!permissionContext.HasPermission(PermissionToken.ManageDatabase)) {
				return StatusSet.Empty;
			}

			// Moderators with lock permissions can edit everything
			if (permissionContext.HasPermission(PermissionToken.LockEntries))
				return allPermissions;

			// Trusted users can edit approved entries
			if (permissionContext.HasPermission(PermissionToken.ApproveEntries))
				return trustedStatusPermissions;

			// Verified artists get trusted permissions for their own entry
			if (entry != null && entry.EntryType == EntryType.Artist 
				&& permissionContext.IsLoggedIn
				&& IsVerifiedFor(permissionContext, entry)) {
				
				return trustedStatusPermissions;

			}

			// Normal user permissions
			if (permissionContext.HasPermission(PermissionToken.ManageDatabase))
				return normalStatusPermissions;

			return StatusSet.Empty;

		}

		public static bool CanDelete(IUserPermissionContext permissionContext, IEntryWithVersions entry) {
			
			if (!permissionContext.IsLoggedIn)
				return false;

			if (permissionContext.HasPermission(PermissionToken.DeleteEntries))
				return true;

			return entry.ArchivedVersionsManager.VersionsBase.All(v => v.Author != null && v.Author.Id == permissionContext.LoggedUserId);

		}

		public static bool CanEdit(IUserPermissionContext permissionContext, SongList songList) {

			if (songList.FeaturedList && CanManageFeaturedLists(permissionContext))
				return true;

			if (permissionContext.HasPermission(PermissionToken.EditAllSongLists))
				return true;

			return (songList.Author.IsTheSameUser(permissionContext.LoggedUser));

		}

		public static bool CanEdit(IUserPermissionContext permissionContext, Comment comment) {

			if (!permissionContext.HasPermission(PermissionToken.CreateComments))
				return false;

			if (permissionContext.HasPermission(PermissionToken.DeleteComments))
				return true;

			return (comment.Author != null && comment.Author.IsTheSameUser(permissionContext.LoggedUser));

		}

		public static bool CanEdit(IUserPermissionContext permissionContext, DiscussionTopic topic) {

			if (!permissionContext.HasPermission(PermissionToken.CreateComments))
				return false;

			if (permissionContext.HasPermission(PermissionToken.DeleteComments))
				return true;

			return (topic.Author != null && topic.Author.IsTheSameUser(permissionContext.LoggedUser));

		}

		/// <summary>
		/// Tests whether the user can edit a specific entry.
		/// The permission depends on both the user's global permissions and entry status.
		/// </summary>
		/// <param name="permissionContext">User permission context. Cannot be null.</param>
		/// <param name="entry">Entry to be checked. Cannot be null.</param>
		/// <returns>True if the user can edit the entry, otherwise false.</returns>
		public static bool CanEdit(IUserPermissionContext permissionContext, IEntryWithStatus entry) {

			ParamIs.NotNull(() => entry);

			return AllowedEntryStatuses(permissionContext, entry).Contains(entry.Status);

		}

		public static bool CanEditAdditionalPermissions(IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => permissionContext);

			return permissionContext.UserGroupId == UserGroupId.Admin;
		}

		public static bool CanEditGroupTo(IUserPermissionContext permissionContext, UserGroupId groupId) {

			ParamIs.NotNull(() => permissionContext);

			return permissionContext.UserGroupId == UserGroupId.Admin || permissionContext.UserGroupId > groupId;

		}

		public static bool CanEditUser(IUserPermissionContext permissionContext, UserGroupId groupId) {

			ParamIs.NotNull(() => permissionContext);

			return CanEditGroupTo(permissionContext, groupId);

		}

		public static bool CanManageFeaturedLists(IUserPermissionContext permissionContext) {

			return permissionContext.HasPermission(PermissionToken.EditFeaturedLists);

		}

		public static void VerifyAccess<T>(IUserPermissionContext permissionContext, T entry, Func<IUserPermissionContext, T, bool> accessCheck) where T : class {

			ParamIs.NotNull(() => entry);

			if (!accessCheck(permissionContext, entry))
				throw new NotAllowedException();

		}

		public static void VerifyDelete(IUserPermissionContext permissionContext, IEntryWithVersions entry) {

			VerifyAccess(permissionContext, entry, CanDelete);

		}

		public static void VerifyEdit(IUserPermissionContext permissionContext, SongList entry) {

			VerifyAccess(permissionContext, entry, CanEdit);

		}

		public static void VerifyEdit(IUserPermissionContext permissionContext, IEntryWithStatus entry) {

			VerifyAccess(permissionContext, entry, CanEdit);

		}

	}
}
