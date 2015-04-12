using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;
using VocaDb.Model.Resources.Messages;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Helpers {

	public class EntryReportNotifier {

		public void SendReportNotification(IRepositoryContext<UserMessage> ctx, 
			ArchivedObjectVersion reportedVersion, 
			string notes, 
			IEntryLinkFactory entryLinkFactory) {
			
			if (reportedVersion == null)
				return;

			var receiver = reportedVersion.Author;

			if (receiver == null)
				return;

			var entry = reportedVersion.EntryBase;

			string body, title;

			using (new ImpersonateUICulture(CultureHelper.GetCultureOrDefault(receiver.Language))) {
				body = EntryReportStrings.EntryVersionReportBody;
				title = EntryReportStrings.EntryVersionReportTitle;		
			}

			var message = string.Format(body, 
				MarkdownHelper.CreateMarkdownLink(entryLinkFactory.GetFullEntryUrl(entry), entry.DefaultName), 
				notes);

			var notification = new UserMessage(receiver, string.Format(title, entry.DefaultName), message, false);
			ctx.Save(notification);

		}

	}

}
