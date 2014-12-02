using System;

namespace VocaDb.Model.Domain.Security {

	[Flags]
	public enum PermissionFlags {

		Nothing				= 0,

		/// <summary>
		/// User is allowed to edit his own profile
		/// </summary>
		EditProfile			= 1,

		/// <summary>
		/// User is allowed to edit the artist/album/song entries and create new entries
		/// </summary>
		ManageDatabase		= 2,

		MergeEntries		= 4,

		DeleteEntries		= 8,

		/// <summary>
		/// User is allowed to restore deleted entries and versions
		/// </summary>
		RestoreEntries		= 16,

		/// <summary>
		/// User is allowed to manage user accounts
		/// </summary>
		ManageUsers			= 32,

		Admin				= 64,

		MikuDbImport		= 128,

		/// <summary>
		/// User is able to manage blocked users
		/// </summary>
		ManageUserBlocks	= 256,

		ViewAuditLog		= 512,

	}

	public static class PermissionFlagsExtender {
	
		public static bool IsSet(this PermissionFlags flags, PermissionFlags flag) {

			return ((flags & flag) == flag);

		}

	}

}
