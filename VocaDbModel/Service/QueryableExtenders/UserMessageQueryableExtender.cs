using System.Linq;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Service.QueryableExtenders {

	public static class UserMessageQueryableExtender {

		/// <summary>
		/// Filter messages by inbox.
		/// </summary>
		/// <param name="query">Query to be filtered. Cannot be null.</param>
		/// <param name="userId">ID of the user whose messages are to be included.</param>
		/// <param name="onlyReceived">
		/// Whether to only allow received messages (in the received and notifications categories).
		/// This affects the <see cref="UserInboxType.Nothing"/> and <see cref="UserInboxType.Sent"/> inboxes.
		/// </param>
		/// <param name="inboxType">Type of inbox to filter by. If <see cref="UserInboxType.Nothing"/>, all messages concerning this user will be included.</param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<UserMessage> WhereInboxIs(this IQueryable<UserMessage> query, int userId, bool onlyReceived, UserInboxType inboxType) {

			if (onlyReceived) {

				if (inboxType == UserInboxType.Nothing)
					return query.Where(m => m.Inbox == UserInboxType.Notifications || m.Inbox == UserInboxType.Received);
				else if (inboxType == UserInboxType.Sent)
					return query.Where(m => false);

			}

			return inboxType == UserInboxType.Nothing ? query : query.Where(m => m.Inbox == inboxType);

		}

		public static IQueryable<UserMessage> WhereIsUnread(this IQueryable<UserMessage> query, bool onlyUnread) {

			return onlyUnread ? query.Where(m => !m.Read) : query;

		}

	}

}
