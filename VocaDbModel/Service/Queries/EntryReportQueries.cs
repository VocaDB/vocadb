using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Queries {

	public class EntryReportQueries {

		public bool CreateReport<TEntry, TReport, TReportType>(IRepositoryContext<TEntry> ctx, 
			IUserPermissionContext permissionContext,
			IEntryLinkFactory entryLinkFactory,
			Expression<Func<TReport, bool>> addExistingEntryFunc, 
			Func<TEntry, User, string, TReport> reportFunc, 
			int entryId, TReportType reportType, string hostname, string notes)
			where TEntry : IEntryWithVersions
			where TReport : EntryReport {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			var loggedUserId = permissionContext.LoggedUserId;
			var existing = ctx.Query<TReport>()
				.Where(addExistingEntryFunc)
				.Where(r => (loggedUserId != 0 && r.User.Id == loggedUserId) || r.Hostname == hostname)
				.FirstOrDefault();

			if (existing != null && !permissionContext.IsLoggedIn)
				return false;

			var entry = ctx.Load(entryId);
			var reporter = ctx.OfType<User>().GetLoggedUserOrNull(permissionContext);
			var report = reportFunc(entry, reporter, notes.Truncate(EntryReport.MaxNotesLength));

			var versionForReport = report.VersionBase;

			if (versionForReport == null) {

				var firstVersion = entry.ArchivedVersionsManager.VersionsBase.FirstOrDefault();

				var oneEditor = firstVersion != null && firstVersion.Author != null &&
					entry.ArchivedVersionsManager.VersionsBase.All(v => Equals(v.Author, firstVersion.Author));

				if (oneEditor)
					versionForReport = firstVersion;

			}

			new EntryReportNotifier().SendReportNotification(ctx.OfType<UserMessage>(), versionForReport, notes, entryLinkFactory);

			if (existing != null)
				return false;

			var msg =  string.Format("reported {0} as {1} ({2})", entryLinkFactory.CreateEntryLink(entry), reportType, HttpUtility.HtmlEncode(notes));
			ctx.AuditLogger.AuditLog(msg.Truncate(200), new AgentLoginData(reporter, hostname));

			ctx.Save(report);
			return true;

		}

	}

}
