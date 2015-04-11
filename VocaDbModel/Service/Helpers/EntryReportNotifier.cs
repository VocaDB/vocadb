using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Helpers {

	public class EntryReportNotifier {

		public void SendReportNotification(IRepositoryContext<UserMessage> ctx, 
			ArchivedObjectVersion reportedVersion, 
			string notes, AgentLoginData reporter, 
			IEntryLinkFactory entryLinkFactory) {
			
			if (reportedVersion == null)
				return;

			var receiver = reportedVersion.Author;

			if (receiver == null)
				return;

			var entry = reportedVersion.EntryBase;

			var message = string.Format("Your edit for '{0}' was reported by {1} with the following message '{2}'.", 
				MarkdownHelper.CreateMarkdownLink(entryLinkFactory.GetFullEntryUrl(entry), entry.DefaultName), 
				reporter.UserNameOrFallback, notes);

			var notification = new UserMessage(receiver, string.Format("Entry report for {0}", entry.DefaultName), message, false);
			ctx.Save(notification);

		}

	}

}
