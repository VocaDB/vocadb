using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Helpers {

	/// <summary>
	/// Sends notification messages to users when they're mentioned in a comment.
	/// </summary>
	public class UserCommentNotifier {

		public void CheckComment(Comment comment, IEntryLinkFactory entryLinkFactory, IRepositoryContext<User> ctx) {

			var userMatches = Regex.Match(comment.Message, @"@(\w+)");

			if (!userMatches.Success)
				return;

			var userNames = userMatches.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();

			var users = ctx.Query().Where(u => u.Active && userNames.Contains(u.Name)).ToArray();

			if (!users.Any())
				return;

			var commentMsg = comment.Message.Truncate(200);
			var msg = string.Format("{0} mentioned you in a comment for {1}\n\n{2}", comment.AuthorName, MarkdownHelper.CreateMarkdownLink(entryLinkFactory.GetFullEntryUrl(comment.Entry), comment.Entry.DefaultName), commentMsg);

			foreach (var user in users) {

				var notification = new UserMessage(user, "Mentioned in a comment", msg, false);
				ctx.OfType<UserMessage>().Save(notification);

			}

		}

	}

}
