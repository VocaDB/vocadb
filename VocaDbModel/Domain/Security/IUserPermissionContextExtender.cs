using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Security
{

	/// <summary>
	/// Extension methods for <see cref="IUserPermissionContext"/>.
	/// </summary>
	public static class IUserPermissionContextExtender
	{

		public static void VerifyEntryDelete<TEntry>(this IUserPermissionContext permissionContext, TEntry entry)
			where TEntry : class, IEntryWithVersions, IEntryWithStatus
		{

			EntryPermissionManager.VerifyDelete(permissionContext, entry);

		}

		public static void VerifyEntryEdit(this IUserPermissionContext permissionContext, IEntryWithStatus entry)
		{

			EntryPermissionManager.VerifyEdit(permissionContext, entry);

		}

		public static void VerifyManageDatabase(this IUserPermissionContext permissionContext)
		{

			permissionContext.VerifyPermission(PermissionToken.ManageDatabase);

		}

		public static void VerifyAccess<T>(this IUserPermissionContext permissionContext, T entry, Func<IUserPermissionContext, T, bool> accessCheck) where T : class
		{

			EntryPermissionManager.VerifyAccess(permissionContext, entry, accessCheck);

		}

		public static void VerifyResourceAccess(this IUserPermissionContext permissionContext, params User[] owners)
		{

			permissionContext.VerifyResourceAccess(owners.Select(o => o.Id));

		}

		public static void VerifyResourceAccess(this IUserPermissionContext permissionContext, params UserContract[] owners)
		{

			permissionContext.VerifyResourceAccess(owners.Select(o => o.Id));

		}

		public static void VerifyResourceAccess(this IUserPermissionContext permissionContext, IEnumerable<int> ownerIds)
		{

			permissionContext.VerifyLogin();

			if (!ownerIds.Contains(permissionContext.LoggedUserId))
				throw new NotAllowedException("You do not have access to this resource.");

		}

	}
}
