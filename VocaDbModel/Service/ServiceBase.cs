using System;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using NLog;
using VocaDb.Model.Domain.Globalization;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Service.Helpers;
using System.Collections.Generic;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Service {

	public abstract class ServiceBase {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private readonly IEntryLinkFactory entryLinkFactory;
		protected const int maxEntryCount = 500;
		private readonly ISessionFactory sessionFactory;
		private readonly IUserPermissionContext permissionContext;

		protected string CreateEntryLink(IEntryBase entry) {
			return EntryLinkFactory.CreateEntryLink(entry);
		}

		private string GetAuditLogMessage(string doingWhat, string who) {

			return string.Format("'{0}' {1}", who, doingWhat);

		}

		protected User GetLoggedUser(ISession session) {

			PermissionContext.VerifyLogin();

			return session.Load<User>(PermissionContext.LoggedUser.Id);

		}

		protected User GetLoggedUserOrDefault(ISession session) {

			return (PermissionContext.LoggedUser != null ? session.Load<User>(PermissionContext.LoggedUser.Id) : null);

		}

		protected IEntryLinkFactory EntryLinkFactory {
			get { return entryLinkFactory; }
		}

		protected ContentLanguagePreference LanguagePreference {
			get { return PermissionContext.LanguagePreference; }
		}

		protected IUserPermissionContext PermissionContext {
			get { return permissionContext; }
		}

		protected ISessionFactory SessionFactory {
			get { return sessionFactory; }
		}

		protected void AddActivityfeedEntry(ISession session, ActivityEntry entry) {

			var latestEntries = session.Query<ActivityEntry>()
				.OrderByDescending(a => a.CreateDate)
				.Take(10)	// time cutoff would be better instead of an arbitrary number of activity entries
				.ToArray();

			if (latestEntries.Any(e => e.IsDuplicate(entry)))
				return;

			session.Save(entry);

		}

		protected void AddEntryEditedEntry(ISession session, Album entry, EntryEditEvent editEvent, ArchivedAlbumVersion archivedVersion) {

			var user = GetLoggedUser(session);
			var activityEntry = new AlbumActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(session, activityEntry);

		}

		protected void AddEntryEditedEntry(ISession session, Artist entry, EntryEditEvent editEvent, ArchivedArtistVersion archivedVersion) {

			var user = GetLoggedUser(session);
			var activityEntry = new ArtistActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(session, activityEntry);

		}

		protected void AddEntryEditedEntry(ISession session, Song entry, EntryEditEvent editEvent, ArchivedSongVersion archivedVersion) {

			var user = GetLoggedUser(session);
			var activityEntry = new SongActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(session, activityEntry);

		}

		/// <summary>
		/// Logs an action in syslog. 
		/// Syslog is saved through NLog to a file.
		/// This override uses the currently logged in user, if any.
		/// </summary>
		/// <param name="doingWhat">What the user was doing.</param>
		protected void SysLog(string doingWhat) {

			SysLog(doingWhat, PermissionContext.Name);

		}

		/// <summary>
		/// Logs an action in syslog. 
		/// Syslog is saved through NLog to a file.
		/// </summary>
		/// <param name="doingWhat">What the user was doing.</param>
		/// <param name="who">Who made the action.</param>
		protected void SysLog(string doingWhat, string who) {

			log.Info(GetAuditLogMessage(doingWhat, who));

		}

		protected void AuditLog(string doingWhat, ISession session, AgentLoginData who, AuditLogCategory category = AuditLogCategory.Unspecified) {

			ParamIs.NotNull(() => session);
			ParamIs.NotNull(() => who);

			SysLog(doingWhat, who.Name);

			var entry = new AuditLogEntry(who, doingWhat, category);

			session.Save(entry);

		}

		protected void AuditLog(string doingWhat, ISession session, string who, AuditLogCategory category = AuditLogCategory.Unspecified) {

			ParamIs.NotNull(() => session);

			SysLog(doingWhat, who);

			var agentLoginData = new AgentLoginData(who);
			var entry = new AuditLogEntry(agentLoginData, doingWhat, category);

			session.Save(entry);

		}

		protected void AuditLog(string doingWhat, ISession session, User user = null, AuditLogCategory category = AuditLogCategory.Unspecified) {

			ParamIs.NotNull(() => session);

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext, user);
			SysLog(doingWhat, agentLoginData.Name);
			var entry = new AuditLogEntry(agentLoginData, doingWhat, category);

			session.Save(entry);

		}

		protected bool DoSnapshot(ArchivedObjectVersion latestVersion, User user) {

			if (latestVersion == null)
				return true;

			return ((((latestVersion.Version + 1) % 5) == 0) || !user.Equals(latestVersion.Author));

		}

		protected void HandleQuery(Action<ISession> func, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var session = OpenSession()) {
					func(session);
				}
			} catch (ObjectNotFoundException x) {
				log.Error(x.Message);
				throw;
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}

		}

		protected T HandleQuery<T>(Func<ISession, T> func, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var session = OpenSession()) {
					return func(session);
				}
			} catch (ObjectNotFoundException x) {
				log.Error(x.Message);
				throw;
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}

		}
		
		protected T HandleTransaction<T>(Func<ISession, T> func, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction()) {

					var val = func(session);
					tx.Commit();
					return val;

				}
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}

		}

		protected T HandleTransaction<T>(Func<ISession, ITransaction, T> func, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction()) {

					var val = func(session, tx);
					tx.Commit();
					return val;

				}
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}

		}

		protected T HandleTransaction<T>(Func<ISession, T> func, IsolationLevel isolationLevel, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction(isolationLevel)) {

					var val = func(session);
					tx.Commit();
					return val;

				}
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}

		}

		protected void HandleTransaction(Action<ISession> func, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction()) {

					func(session);
					tx.Commit();

				}
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}

		}

		protected void HandleTransaction(Action<ISession> func, IsolationLevel isolationLevel, string failMsg = "Unexpected NHibernate error") {

			try {
				using (var session = OpenSession())
				using (var tx = session.BeginTransaction(isolationLevel)) {

					func(session);
					tx.Commit();

				}
			} catch (HibernateException x) {
				log.Error(x, failMsg);
				throw;
			}

		}

		protected ISession OpenSession() {
			return sessionFactory.OpenSession();
		}

		protected ServiceBase(
			ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) {

			ParamIs.NotNull(() => sessionFactory);

			this.sessionFactory = sessionFactory;
			this.permissionContext = permissionContext;
			this.entryLinkFactory = entryLinkFactory;

		}

		protected void DeleteEntity<TEntity>(int id, PermissionToken permissionFlags, bool skipLog = false) {

			var typeName = typeof(TEntity).Name;
			SysLog(string.Format("is about to delete {0} with Id {1}", typeName, id));
			PermissionContext.VerifyPermission(permissionFlags);

			HandleTransaction(session => {

				var entity = session.Load<TEntity>(id);

				if (!skipLog)
					AuditLog("deleting " + entity, session);
				else
					SysLog("deleting " + entity);

				session.Delete(entity);

			}, "Unable to delete " + typeName);

		}

		protected void UpdateEntity<TEntity>(int id, Action<TEntity> func, PermissionToken permissionFlags, bool skipLog = false) {

			var typeName = typeof(TEntity).Name;

			SysLog(string.Format("is about to update {0} with Id {1}", typeName, id));
			PermissionContext.VerifyPermission(permissionFlags);

			HandleTransaction(session => {

				var entity = session.Load<TEntity>(id);

				if (!skipLog)
					AuditLog("updating " + entity, session);
				else
					SysLog("updating " + entity);

				func(entity);

				session.Update(entity);

			}, "Unable to update " + typeName);

		}

		protected void UpdateEntity<TEntity>(int id, Action<ISession, TEntity> func, PermissionToken permissionFlags, bool skipLog = false) {

			var typeName = typeof(TEntity).Name;

			SysLog(string.Format("is about to update {0} with Id {1}", typeName, id));
			PermissionContext.VerifyPermission(permissionFlags);

			HandleTransaction(session => {

				var entity = session.Load<TEntity>(id);

				if (!skipLog)
					AuditLog("updating " + entity, session);
				else
					SysLog("updating " + entity);

				func(session, entity);

				session.Update(entity);

			}, "Unable to update " + typeName);

		}

		protected void VerifyEntryEdit(IEntryWithStatus entry) {

			EntryPermissionManager.VerifyEdit(PermissionContext, entry);

		}

		protected void VerifyManageDatabase() {

			PermissionContext.VerifyPermission(PermissionToken.ManageDatabase);

		}

		protected void VerifyResourceAccess(params User[] owners) {

			VerifyResourceAccess(owners.Select(o => o.Id));
				
		}

		protected void VerifyResourceAccess(params UserContract[] owners) {

			VerifyResourceAccess(owners.Select(o => o.Id));

		}

		private void VerifyResourceAccess(IEnumerable<int> ownerIds) {

			PermissionContext.VerifyLogin();

			if (!ownerIds.Contains(PermissionContext.LoggedUser.Id))
				throw new NotAllowedException("You do not have access to this resource.");

		}

	}

	/// <summary>
	/// Match mode for name queries.
	/// </summary>
	public enum NameMatchMode {

		/// <summary>
		/// Automatically choose match mode based on query term length.
		/// For longer queries this is generally Words, for shorter StartsWith.
		/// </summary>
		Auto,

		/// <summary>
		/// Always partial matching.
		/// Wildcards are allowed.
		/// </summary>
		Partial,

		/// <summary>
		/// Prefix search.
		/// </summary>
		StartsWith,

		/// <summary>
		/// Exact (strict) match (usually still case-insensitive).
		/// Wildcards are not allowed.
		/// </summary>
		Exact,

		/// <summary>
		/// Allow breaking the search string into words separated by whitespace.
		/// The order of words does not matter.
		/// </summary>
		Words

	}

	public static class PartialFindResult {

		public static PartialFindResult<T> Create<T>(T[] items, int totalCount) {
			return new PartialFindResult<T>(items, totalCount);
		}

	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PartialFindResult<T> {

		public PartialFindResult() { 
			Items = new T[] {};
		} 

		public PartialFindResult(T[] items, int totalCount) {

			Items = items;
			TotalCount = totalCount;

		}

		public PartialFindResult(T[] items, int totalCount, string term)
			: this(items, totalCount) {

			Term = term;

		}

		[DataMember]
		public T[] Items { get; set; }

		[DataMember]
		public string Term { get; set; }

		[DataMember]
		public int TotalCount { get; set; }

	}


}
