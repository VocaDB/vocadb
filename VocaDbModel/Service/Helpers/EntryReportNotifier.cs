using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;
using VocaDb.Model.Resources.Messages;

namespace VocaDb.Model.Service.Helpers {

	public class EntryReportNotifier {

		public void SendReportNotification(IDatabaseContext<UserMessage> ctx, 
			ArchivedObjectVersion reportedVersion, 
			string notes, 
			IEntryLinkFactory entryLinkFactory,
			string reportName) {
			
			if (reportedVersion == null)
				return;

			var receiver = reportedVersion.Author;

			if (receiver == null)
				return;

			var entry = reportedVersion.EntryBase;

			string body, title;

			using (new ImpersonateUICulture(CultureHelper.GetCultureOrDefault(receiver.LanguageOrLastLoginCulture))) {
				body = EntryReportStrings.EntryVersionReportBody;
				title = EntryReportStrings.EntryVersionReportTitle;		
			}

			// Report type name + notes
			string notesAndName = notes;

			if (!string.IsNullOrEmpty(reportName) && !string.IsNullOrEmpty(notes)) {
				notesAndName = string.Format("{0} ({1})", reportName, notes);
			} else if (string.IsNullOrEmpty(notes)) {
				notesAndName = reportName;
			}

			var message = string.Format(body, 
				MarkdownHelper.CreateMarkdownLink(entryLinkFactory.GetFullEntryUrl(entry), entry.DefaultName), 
				notesAndName);

			var notification = new UserMessage(receiver, string.Format(title, entry.DefaultName), message, false);
			ctx.Save(notification);

		}

	}

}
