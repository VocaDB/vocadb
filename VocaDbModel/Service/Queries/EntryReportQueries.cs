using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Queries {

	public class EntryReportQueries {

		public bool CreateReport<TEntry, TReport, TReportType>(IDatabaseContext<TEntry> ctx, 
			IUserPermissionContext permissionContext,
			IEntryLinkFactory entryLinkFactory,
			Expression<Func<TReport, bool>> addExistingEntryFunc, 
			Func<TEntry, User, string, TReport> reportFunc, 
			Func<string> reportNameFunc, 
			int entryId, TReportType reportType, string hostname, string notes)
			where TEntry : IEntryWithVersions
			where TReport : EntryReport {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			var msg = string.Format("creating report for {0} [{1}] as {2}", typeof(TEntry).Name, entryId, reportType);
			ctx.AuditLogger.SysLog(msg.Truncate(200), hostname);

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

			// Reported version. If a specific version was reported the author is already defined.
			var versionForReport = report.VersionBase;

			if (versionForReport == null) {

				// Get first version, check if all following edits were made by the same user OR the reporter
				var firstVersion = entry.ArchivedVersionsManager.VersionsBase.FirstOrDefault(
					ver => ver.Author != null && !ver.Author.Equals(reporter));

				var oneEditor = firstVersion != null && firstVersion.Author != null &&
					entry.ArchivedVersionsManager.VersionsBase.All(ver =>
						Equals(ver.Author, reporter) ||  // Editor is reporter
						firstVersion.Author.Equals(ver.Author)); // Editor is the creator

				if (oneEditor)
					versionForReport = firstVersion; // TODO: need to include report type in notification

			}

			// Get translated report type name
			string reportName = null;
			if (versionForReport != null && versionForReport.Author != null) {
				using (new ImpersonateUICulture(CultureHelper.GetCultureOrDefault(versionForReport.Author.LanguageOrLastLoginCulture))) {
					reportName = reportNameFunc();
				}				
			}

			new EntryReportNotifier().SendReportNotification(ctx.OfType<UserMessage>(), versionForReport, notes, entryLinkFactory, reportName);

			if (existing != null)
				return false;

			msg =  string.Format("reported {0} as {1} ({2})", entryLinkFactory.CreateEntryLink(entry), reportType, HttpUtility.HtmlEncode(notes));
			ctx.AuditLogger.AuditLog(msg.Truncate(200), new AgentLoginData(reporter, hostname));

			ctx.Save(report);
			return true;

		}

	}

}
