using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	public abstract class QueriesBase<TRepo, TEntity> where TRepo : class, IRepository<TEntity> {

		protected readonly IUserPermissionContext permissionContext;
		protected readonly TRepo repository;

		protected ContentLanguagePreference LanguagePreference {
			get { return permissionContext.LanguagePreference; }
		}

		protected IUserPermissionContext PermissionContext {
			get { return permissionContext; }
		}

		protected void AddActivityfeedEntry(IRepositoryContext<ActivityEntry> ctx, ActivityEntry entry) {

			var latestEntries = ctx.Query()
				.OrderByDescending(a => a.CreateDate)
				.Take(10)	// time cutoff would be better instead of an arbitrary number of activity entries
				.ToArray();

			if (latestEntries.Any(e => e.IsDuplicate(entry)))
				return;

			ctx.Save(entry);

		}

		protected void AddEntryEditedEntry(IRepositoryContext<ActivityEntry> ctx, Album entry, EntryEditEvent editEvent, ArchivedAlbumVersion archivedVersion) {

			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);
			var activityEntry = new AlbumActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(ctx, activityEntry);

		}

		protected void AddEntryEditedEntry(IRepositoryContext<ActivityEntry> ctx, Artist entry, EntryEditEvent editEvent, ArchivedArtistVersion archivedVersion) {

			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);
			var activityEntry = new ArtistActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(ctx, activityEntry);

		}

		protected void AddEntryEditedEntry(IRepositoryContext<ActivityEntry> ctx, Song entry, EntryEditEvent editEvent, ArchivedSongVersion archivedVersion) {

			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);
			var activityEntry = new SongActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(ctx, activityEntry);

		}

		protected bool DoSnapshot(ArchivedObjectVersion latestVersion, User user) {

			if (latestVersion == null)
				return true;

			return ((((latestVersion.Version + 1) % 5) == 0) || !user.Equals(latestVersion.Author));

		}

		protected void VerifyEntryEdit(IEntryWithStatus entry) {

			EntryPermissionManager.VerifyEdit(PermissionContext, entry);

		}

		protected void VerifyManageDatabase() {

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

		}

		protected void VerifyResourceAccess(params IUser[] owners) {

			VerifyResourceAccess(owners.Select(o => o.Id));

		}

		private void VerifyResourceAccess(IEnumerable<int> ownerIds) {

			PermissionContext.VerifyLogin();

			if (!ownerIds.Contains(PermissionContext.LoggedUser.Id))
				throw new NotAllowedException("You do not have access to this resource.");

		}

		protected QueriesBase(TRepo repository, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => repository);
			ParamIs.NotNull(() => permissionContext);

			this.repository = repository;
			this.permissionContext = permissionContext;

		}

		/// <summary>
		/// Runs an unit of work that queries the database without saving anything. No explicit transaction will be opened.
		/// </summary>
		/// <typeparam name="TResult">Type of the result.</typeparam>
		/// <param name="func">Function running the unit of work. Cannot be null.</param>
		/// <param name="failMsg">Failure message. Cannot be null.</param>
		/// <returns>Result. Can be null.</returns>
		public TResult HandleQuery<TResult>(Func<IRepositoryContext<TEntity>, TResult> func, string failMsg = "Unexpected database error") {
			return repository.HandleQuery(func, failMsg);
		}

		/// <summary>
		/// Runs an unit of work that does not return anything, inside an explicit transaction.
		/// </summary>
		/// <param name="func">Function running the unit of work. Cannot be null.</param>
		/// <param name="failMsg">Failure message. Cannot be null.</param>
		/// <returns>Result. Can be null.</returns>
		public void HandleTransaction(Action<IRepositoryContext<TEntity>> func, string failMsg = "Unexpected database error") {
			repository.HandleTransaction(func, failMsg);
		}

		/// <summary>
		/// Runs an unit of work that queries the database, inside an explicit transaction.
		/// </summary>
		/// <typeparam name="TResult">Type of the result.</typeparam>
		/// <param name="func">Function running the unit of work. Cannot be null.</param>
		/// <param name="failMsg">Failure message. Cannot be null.</param>
		/// <returns>Result. Can be null.</returns>
		public TResult HandleTransaction<TResult>(Func<IRepositoryContext<TEntity>, TResult> func, string failMsg = "Unexpected database error") {
			return repository.HandleTransaction(func, failMsg);
		}

	}

}