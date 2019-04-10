using System;
using System.Linq;
using System.Web;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Queries {

	/// <summary>
	/// Creates <see cref="EntryReport"/>s.
	/// </summary>
	/// <remarks>
	/// Note: this class is tested through <see cref="SongQueriesTests"/>.
	/// </remarks>
	public class EntryReportQueries {

		/// <summary>
		/// Creates entry report.
		/// </summary>
		/// <typeparam name="TEntry">Entry type.</typeparam>
		/// <typeparam name="TReport">Report type (class).</typeparam>
		/// <typeparam name="TReportType">Report subtype (enum).</typeparam>
		/// <param name="ctx">Query context.</param>
		/// <param name="permissionContext">Permission context.</param>
		/// <param name="entryLinkFactory">Entry link factory.</param>
		/// <param name="reportFunc">Factory for creating report.</param>
		/// <param name="reportNameFunc">Report name getter.</param>
		/// <param name="entryId">Entry Id.</param>
		/// <param name="reportType">Report subtype.</param>
		/// <param name="hostname">Reporter's hostname (IP address).</param>
		/// <param name="notes">Report notes, if any. Can be empty.</param>
		/// <returns>Tuple informing whether report was created, and report Id.</returns>
		public (bool created, int reportId) CreateReport<TEntry, TReport, TReportType>(
			IDatabaseContext<TEntry> ctx, 
			IUserPermissionContext permissionContext,
			IEntryLinkFactory entryLinkFactory,
			Func<TEntry, User, string, TReport> reportFunc, 
			Func<string> reportNameFunc, 
			int entryId, 
			TReportType reportType, 
			string hostname, 
			string notes)
			where TEntry : class, IEntryWithVersions, IEntryWithNames
			where TReport : GenericEntryReport<TEntry, TReportType>
			where TReportType: struct, Enum {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			var msg = string.Format("creating report for {0} [{1}] as {2}", typeof(TEntry).Name, entryId, reportType);
			ctx.AuditLogger.SysLog(msg, hostname);

			var loggedUserId = permissionContext.LoggedUserId;
			var existing = ctx.Query<TReport>()				
				.Where(r => r.Entry.Id == entryId && (loggedUserId != 0 && r.User.Id == loggedUserId) || r.Hostname == hostname)
				.OrderByDescending(r => r.Created)
				.ThenByDescending(r => r.Id)
				.FirstOrDefault();

			var duplicate = existing != null;

			if (duplicate && (!permissionContext.IsLoggedIn || existing.Status == ReportStatus.Open))
				return (false, existing.Id);

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

			msg =  string.Format("reported {0} as {1} ({2})", entryLinkFactory.CreateEntryLink(entry), reportType, HttpUtility.HtmlEncode(notes));
			ctx.AuditLogger.AuditLog(msg.Truncate(200), new AgentLoginData(reporter, hostname));

			ctx.Save(report);
			return (true, report.Id);

		}

	}

}
