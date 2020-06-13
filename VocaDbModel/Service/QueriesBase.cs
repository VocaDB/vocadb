using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Service {

	public abstract class QueriesBase<TRepo, TEntity> 
		where TRepo : class, IRepository<TEntity> 
		where TEntity : class, IDatabaseObject {

		protected readonly IUserPermissionContext permissionContext;
		protected readonly TRepo repository;

		protected ContentLanguagePreference LanguagePreference {
			get { return permissionContext.LanguagePreference; }
		}

		protected IUserPermissionContext PermissionContext {
			get { return permissionContext; }
		}

		protected void AddActivityfeedEntry(IDatabaseContext<ActivityEntry> ctx, ActivityEntry entry) {

			new Queries.ActivityEntryQueries(ctx, PermissionContext).AddActivityfeedEntry(entry);

		}

		protected async Task AddActivityfeedEntryAsync(IDatabaseContext<ActivityEntry> ctx, ActivityEntry entry) {

			await new Queries.ActivityEntryQueries(ctx, PermissionContext).AddActivityfeedEntryAsync(entry);

		}

		protected void AddActivityfeedEntry(IDatabaseContext<ActivityEntry> ctx, Func<User, ActivityEntry> entryFunc) {

			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);
			AddActivityfeedEntry(ctx, entryFunc(user));

		}

		protected void AddEntryEditedEntry(IDatabaseContext<ActivityEntry> ctx, Album entry, EntryEditEvent editEvent, ArchivedAlbumVersion archivedVersion) {

			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);
			var activityEntry = new AlbumActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(ctx, activityEntry);

		}

		protected void AddEntryEditedEntry(IDatabaseContext<ActivityEntry> ctx, Artist entry, EntryEditEvent editEvent, ArchivedArtistVersion archivedVersion) {

			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);
			var activityEntry = new ArtistActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(ctx, activityEntry);

		}

		protected void AddEntryEditedEntry(IDatabaseContext<ActivityEntry> ctx, ArchivedReleaseEventVersion archivedVersion) {

			AddActivityfeedEntry(ctx, user => new ReleaseEventActivityEntry(archivedVersion.ReleaseEvent, archivedVersion.EditEvent, user, archivedVersion));

		}

		protected void AddEntryEditedEntry(IDatabaseContext<ActivityEntry> ctx, Song entry, EntryEditEvent editEvent, ArchivedSongVersion archivedVersion) {

			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);
			var activityEntry = new SongActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(ctx, activityEntry);

		}

		protected async Task AddEntryEditedEntryAsync(IDatabaseContext<ActivityEntry> ctx, Song entry, EntryEditEvent editEvent, ArchivedSongVersion archivedVersion) {

			var user = await ctx.OfType<User>().GetLoggedUserAsync(PermissionContext);
			var activityEntry = new SongActivityEntry(entry, editEvent, user, archivedVersion);
			await AddActivityfeedEntryAsync(ctx, activityEntry);

		}

		protected void AddEntryEditedEntry(IDatabaseContext<ActivityEntry> ctx, SongList entry, EntryEditEvent editEvent, ArchivedSongListVersion archivedVersion) {

			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);
			var activityEntry = new SongListActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(ctx, activityEntry);

		}

		protected void AddEntryEditedEntry(IDatabaseContext<ActivityEntry> ctx, Tag entry, EntryEditEvent editEvent, ArchivedTagVersion archivedVersion) {

			new Queries.ActivityEntryQueries(ctx, PermissionContext).AddEntryEditedEntry(entry, editEvent, archivedVersion);

		}
		
		protected void AddEntryEditedEntry(IDatabaseContext<ActivityEntry> ctx, Venue entry, EntryEditEvent editEvent, ArchivedVenueVersion archivedVersion) {

			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);
			var activityEntry = new VenueActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(ctx, activityEntry);

		}

		protected void AuditLog(string doingWhat, IDatabaseContext<TEntity> session, AgentLoginData who, AuditLogCategory category = AuditLogCategory.Unspecified) {
			session.AuditLogger.AuditLog(doingWhat, who, category);
		}

		protected void AuditLog(string doingWhat, IDatabaseContext<TEntity> session, string who, AuditLogCategory category = AuditLogCategory.Unspecified) {
			session.AuditLogger.AuditLog(doingWhat, who, category);
		}

		protected void AuditLog(string doingWhat, IDatabaseContext<TEntity> session, User user = null, AuditLogCategory category = AuditLogCategory.Unspecified) {
			session.AuditLogger.AuditLog(doingWhat, user, category);
		}

		protected bool DoSnapshot(IEntryWithVersions entry, IDatabaseContext ctx) {

			var latestVersion = entry.ArchivedVersionsManager.GetLatestVersion();
			var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);

			return DoSnapshot(latestVersion, user);

		}

		protected bool DoSnapshot(ArchivedObjectVersion latestVersion, User user) {

			if (latestVersion == null)
				return true;

			return ((((latestVersion.Version + 1) % 5) == 0) || !user.Equals(latestVersion.Author));

		}

		protected User GetLoggedUser(IDatabaseContext session) {
			return session.OfType<User>().GetLoggedUser(PermissionContext);
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
		public TResult HandleQuery<TResult>(Func<IDatabaseContext<TEntity>, TResult> func, string failMsg = "Unexpected database error") {
			return repository.HandleQuery(func, failMsg);
		}

		/// <summary>
		/// Runs an unit of work that does not return anything, inside an explicit transaction.
		/// </summary>
		/// <param name="func">Function running the unit of work. Cannot be null.</param>
		/// <param name="failMsg">Failure message. Cannot be null.</param>
		/// <returns>Result. Can be null.</returns>
		public void HandleTransaction(Action<IDatabaseContext<TEntity>> func, string failMsg = "Unexpected database error") {
			repository.HandleTransaction(func, failMsg);
		}

		/// <summary>
		/// Runs an unit of work that queries the database, inside an explicit transaction.
		/// </summary>
		/// <typeparam name="TResult">Type of the result.</typeparam>
		/// <param name="func">Function running the unit of work. Cannot be null.</param>
		/// <param name="failMsg">Failure message. Cannot be null.</param>
		/// <returns>Result. Can be null.</returns>
		public TResult HandleTransaction<TResult>(Func<IDatabaseContext<TEntity>, TResult> func, string failMsg = "Unexpected database error") {
			return repository.HandleTransaction(func, failMsg);
		}

	}

}