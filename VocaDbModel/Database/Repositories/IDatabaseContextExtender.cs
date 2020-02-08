using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Model.Database.Repositories {

	/// <summary>
	/// Extension methods for <see cref="IDatabaseContext"/>.
	/// </summary>
	public static class IDatabaseContextExtender {

		public static AgentLoginData CreateAgentLoginData<T>(this IDatabaseContext<T> ctx, IUserPermissionContext permissionContext, User user = null) {

			if (user != null)
				return new AgentLoginData(user);

			if (permissionContext.IsLoggedIn) {

				user = ctx.OfType<User>().GetLoggedUser(permissionContext);
				return new AgentLoginData(user);

			} else {

				return new AgentLoginData(permissionContext.Name);

			}

		}

		public static void Delete<T>(this IDatabaseContext ctx, T obj) where T : class, IDatabaseObject {
			ctx.OfType<T>().Delete(obj);
		}

		public static void Delete<T, T2>(this IDatabaseContext<T> ctx, T2 obj) where T2 : class, IDatabaseObject {
			ctx.OfType<T2>().Delete(obj);
		}

		public static void DeleteAll<T, T2>(this IDatabaseContext<T> ctx, IEnumerable<T2> objs) 
			where T2 : class, IDatabaseObject {

			var ctxTyped = ctx.OfType<T2>();

			foreach (var obj in objs)
				ctxTyped.Delete(obj);

		}

		public static User GetLoggedUser(this IDatabaseContext<User> ctx, IUserPermissionContext permissionContext) {

			permissionContext.VerifyLogin();

			return ctx.Load(permissionContext.LoggedUserId);

		}

		public static User GetLoggedUserOrNull(this IDatabaseContext<User> ctx, IUserPermissionContext permissionContext) {

			return (permissionContext.LoggedUser != null ? ctx.Load(permissionContext.LoggedUser.Id) : null);

		}

		public static T2 Load<T2>(this IDatabaseContext ctx, object id) where T2 : class, IDatabaseObject 
			=> ctx.OfType<T2>().Load(id);

		public static Task<T2> LoadAsync<T2>(this IDatabaseContext ctx, object id) where T2 : class, IDatabaseObject
			=> ctx.OfType<T2>().LoadAsync(id);

		public static T2 Load<T, T2>(this IDatabaseContext<T> ctx, object id) where T2 : class, IDatabaseObject
			=> ctx.OfType<T2>().Load(id);

		public static T LoadEntry<T>(this IDatabaseContext ctx, IEntryWithIntId entry) where T : class, IDatabaseObject
			=> ctx.Load<T>(entry.Id);

		public static IQueryable<T2> LoadMultiple<T2>(this IDatabaseContext ctx, IEnumerable<int> ids) where T2 : class, IEntryWithIntId
			=> ctx.OfType<T2>().Query().WhereIdIn(ids);

		/// <summary>
		/// Loads an entry based on a reference, or returns null if the reference is null or points to an entry that shouldn't exist (Id is 0).
		/// </summary>
		/// <typeparam name="T">Type of entry to be loaded.</typeparam>
		/// <param name="ctx">Repository context. Cannot be null.</param>
		/// <param name="entry">Entry reference. Can be null in which case null is returned.</param>
		/// <returns>Reference to the loaded entry. Can be null if <paramref name="entry"/> is null or Id is 0.</returns>
		public static T NullSafeLoad<T>(this IDatabaseContext<T> ctx, IEntryWithIntId entry) {
			return entry != null && entry.Id != 0 ? ctx.Load(entry.Id) : default(T);
		}

		public static T NullSafeLoad<T>(this IDatabaseContext<T> ctx, int id) {
			return id != 0 ? ctx.Load(id) : default;
		}

		public static T NullSafeLoad<T>(this IDatabaseContext ctx, int id) where T : class, IDatabaseObject {
			return id != 0 ? ctx.Load<T>(id) : default;
		}

		public static T NullSafeLoad<T>(this IDatabaseContext ctx, IEntryWithIntId entry) where T : class, IDatabaseObject {
			return entry != null && entry.Id != 0 ? ctx.Load<T>(entry.Id) : default;
		}

		public static void Sync<T>(this IDatabaseContext<T> ctx, CollectionDiff<T, T> diff) {

			ParamIs.NotNull(() => ctx);
			ParamIs.NotNull(() => diff);

			foreach (var n in diff.Removed)
				ctx.Delete(n);

			foreach (var n in diff.Added)
				ctx.Save(n);

			foreach (var n in diff.Unchanged)
				ctx.Update(n);

		}

		/// <summary>
		/// Synchronizes the given changes to database, meaning calls
		/// insert, update and delete as appropriate.
		/// </summary>
		/// <typeparam name="T">Context type.</typeparam>
		/// <typeparam name="T2">Element type.</typeparam>
		/// <param name="ctx">Database context.</param>
		/// <param name="diff">Element diff.</param>
		/// <returns><paramref name="diff"/></returns>
		public static CollectionDiff<T2, T2> Sync<T, T2>(this IDatabaseContext<T> ctx, CollectionDiff<T2, T2> diff)
			where T2 : class, IDatabaseObject {

			ParamIs.NotNull(() => ctx);

			Sync<T2>(ctx.OfType<T2>(), diff);
			return diff;

		}

		public static void Sync<T>(this IDatabaseContext<T> ctx, CollectionDiffWithValue<T, T> diff) {

			ParamIs.NotNull(() => ctx);
			ParamIs.NotNull(() => diff);

			foreach (var n in diff.Removed)
				ctx.Delete(n);

			foreach (var n in diff.Added)
				ctx.Save(n);

			foreach (var n in diff.Edited)
				ctx.Update(n);

		}

		public static T2 Save<T, T2>(this IDatabaseContext<T> ctx, T2 obj) where T2 : class, IDatabaseObject => 
			ctx.OfType<T2>().Save(obj);

		public static T Save<T>(this IDatabaseContext ctx, T obj) where T : class, IDatabaseObject
			=> ctx.OfType<T>().Save(obj);

		public static void Update<T>(this IDatabaseContext ctx, T obj) where T : class, IDatabaseObject
			=> ctx.OfType<T>().Update(obj);

		public static void Update<T, T2>(this IDatabaseContext<T> ctx, T2 obj) where T2 : class, IDatabaseObject
			=> ctx.OfType<T2>().Update(obj);

		public static Task UpdateAsync<T, T2>(this IDatabaseContext<T> ctx, T2 obj) where T2 : class, IDatabaseObject
			=> ctx.OfType<T2>().UpdateAsync(obj);

	}

}
