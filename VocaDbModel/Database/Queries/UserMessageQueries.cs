using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Model.Database.Queries {

	/// <summary>
	/// Queries for <see cref="UserMessage"/>.
	/// </summary>
	public class UserMessageQueries : QueriesBase<IUserMessageRepository, UserMessage> {

		public UserMessageQueries(IUserMessageRepository repository, IUserPermissionContext permissionContext) 
			: base(repository, permissionContext) {}

		/// <summary>
		/// Permanently deletes a message by Id.
		/// </summary>
		/// <param name="messageId">Id of the message to be deleted.</param>
		public void Delete(int messageId) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			repository.HandleTransaction(ctx => {

				var msg = ctx.Load(messageId);

				VerifyResourceAccess(msg.User);

				if (msg.Sender != null)
					msg.Sender.SentMessages.Remove(msg);

				msg.Receiver.ReceivedMessages.Remove(msg);
				ctx.Delete(msg);

				ctx.AuditLogger.SysLog(string.Format("deleted {0}", msg));

			});

		}

		public UserMessageContract Get(int messageId, IUserIconFactory iconFactory) {

			PermissionContext.VerifyPermission(PermissionToken.EditProfile);

			return repository.HandleTransaction(ctx => {

				var msg = ctx.Load(messageId);

				VerifyResourceAccess(msg.User);

				if (!msg.Read && PermissionContext.LoggedUserId == msg.Receiver.Id) {
					msg.Read = true;
					ctx.Update(msg);
				}

				return new UserMessageContract(msg, iconFactory, includeBody: true);

			});

		}

		public PartialFindResult<UserMessageContract> GetList(int id, PagingProperties paging, UserInboxType inboxType, bool unread, IUserIconFactory iconFactory) {

			PermissionContext.VerifyResourceAccess(new[] { id });

			return HandleQuery(ctx => {

				var query = ctx.Query()
					.Where(u => u.User.Id == id)
					.WhereInboxIs(inboxType, unread)
					.WhereIsUnread(unread);

				var messages = query
					.OrderByDescending(m => m.Created)
					.Paged(paging, true)
					.ToArray()
					.Select(m => new UserMessageContract(m, iconFactory))
					.ToArray();

				var count = paging.GetTotalCount ? query.Count() : 0;

				return new PartialFindResult<UserMessageContract>(messages, count);

			});

		}
	}

}