using NHibernate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Service.Helpers {

	public static class SessionHelper {

		public static AgentLoginData CreateAgentLoginData(ISession session, IUserPermissionContext permissionContext, User user = null) {

			if (user != null)
				return new AgentLoginData(user);

			if (permissionContext.LoggedUser != null) {

				user = session.Load<User>(permissionContext.LoggedUser.Id);
				return new AgentLoginData(user);

			} else {
				
				return new AgentLoginData(permissionContext.Name);

			}			

		}

		public static void RestoreObjectRefs<TExisting, TEntry>(ISession session, IList<string> warnings, IEnumerable<TExisting> existing,
			IEnumerable<ObjectRefContract> objRefs, Func<TExisting, ObjectRefContract, bool> equality, 
			Func<TEntry, TExisting> createEntryFunc, Action<TExisting> deleteFunc)
			where TEntry : class where TExisting : class {

			RestoreObjectRefs<TExisting, TEntry, ObjectRefContract>(session, warnings, existing, objRefs, equality, (entry, ex) 
				=> createEntryFunc(entry), deleteFunc);

		}

		public static CollectionDiff<TExisting, TObjRef> RestoreObjectRefs<TExisting, TEntry, TObjRef>(ISession session, IList<string> warnings, IEnumerable<TExisting> existing,
			IEnumerable<TObjRef> objRefs, Func<TExisting, TObjRef, bool> equality,
			Func<TEntry, TObjRef, TExisting> createEntryFunc, Action<TExisting> deleteFunc) 
			where TObjRef : ObjectRefContract where TEntry : class where TExisting : class {

			if (objRefs == null)
				objRefs = Enumerable.Empty<TObjRef>();

			var diff = CollectionHelper.Diff(existing, objRefs, equality);

			// If the reference existed in the version being restored, but doesn't exist in the current version.
			foreach (var objRef in diff.Added) {

				// If the reference points to an associated root entity in the database, attempt to restore the reference.
				if (objRef.Id != 0) {

					var entry = session.Get<TEntry>(objRef.Id);

					// Root entity still found in the database, create the link object.
					if (entry != null) {
						var added = createEntryFunc(entry, objRef);
						if (added != null)
							session.Save(added);
					} else {
						warnings.Add("Referenced " + typeof(TEntry).Name + " " + objRef + " not found");
					}

				} else {

					// For composite child objects just recreate the object since it's not a root entity.
					var added = createEntryFunc(null, objRef);
					if (added != null)
						session.Save(added);

				}

			}

			// If the reference did not exist in the version being restored, but exists in the current version, delete the link object.
			foreach (var removed in diff.Removed) {
				deleteFunc(removed);
				session.Delete(removed);
			}

			return diff;

		}

		/// <summary>
		/// Restores a weak root entity reference where the target might be deleted already.
		/// In this case a warning will be issued but otherwise the reference can be ignored.
		/// </summary>
		/// <typeparam name="TEntry">Type of entry to be loaded.</typeparam>
		/// <param name="session">Session.</param>
		/// <param name="warnings">List of warnings. Cannot be null.</param>
		/// <param name="objRef">Reference to the target. Can be null in which case null is returned.</param>
		/// <returns>The restored object reference. Can be null if the reference was null originally, or the target is deleted.</returns>
		public static TEntry RestoreWeakRootEntityRef<TEntry>(ISession session, IList<string> warnings, ObjectRefContract objRef) where TEntry : class {
			
			if (objRef == null)
				return null;

			var obj = session.Get<TEntry>(objRef.Id);
			if (obj == null) {
				warnings.Add(string.Format("Referenced {0} {1} not found", typeof(TEntry).Name, objRef));				
			}

			return obj;

		}

		public static void Sync<T>(ISession session, CollectionDiff<T,T> diff) {

			ParamIs.NotNull(() => session);
			ParamIs.NotNull(() => diff);

			foreach (var n in diff.Removed)
				session.Delete(n);

			foreach (var n in diff.Added)
				session.Save(n);

			foreach (var n in diff.Unchanged)
				session.Update(n);

		}

		public static void Sync<T>(ISession session, CollectionDiffWithValue<T, T> diff) {

			ParamIs.NotNull(() => session);
			ParamIs.NotNull(() => diff);

			foreach (var n in diff.Removed)
				session.Delete(n);

			foreach (var n in diff.Added)
				session.Save(n);

			foreach (var n in diff.Edited)
				session.Update(n);

		}

	}
}
