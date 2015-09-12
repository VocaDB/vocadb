using System;
using System.Linq;
using System.Web;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Queries {

	public class CommentQueries<T, TEntry> where T : GenericComment<TEntry> where TEntry : class, IEntryWithNames {

		private readonly IRepositoryContext<T> ctx;
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IUserPermissionContext permissionContext;
		private readonly IUserIconFactory userIconFactory;

		public CommentQueries(IRepositoryContext<T> ctx, IUserPermissionContext permissionContext, IUserIconFactory userIconFactory, IEntryLinkFactory entryLinkFactory) {
			this.ctx = ctx;
			this.entryLinkFactory = entryLinkFactory;
			this.permissionContext = permissionContext;
			this.userIconFactory = userIconFactory;
		}

		public CommentForApiContract Create(int entryId, CommentForApiContract contract, Func<TEntry, CommentForApiContract, AgentLoginData, T> fac) {
			
			permissionContext.VerifyPermission(PermissionToken.CreateComments);

			if (contract.Author == null || contract.Author.Id != permissionContext.LoggedUserId) {
				throw new NotAllowedException("Can only post as self");
			}
			
			var entry = ctx.OfType<TEntry>().Load(entryId);
			var agent = ctx.OfType<User>().CreateAgentLoginData(permissionContext, ctx.OfType<User>().Load(contract.Author.Id));

			var comment = fac(entry, contract, agent);

			ctx.Save(comment);

			ctx.AuditLogger.AuditLog(string.Format("creating comment for {0}: '{1}'", 
				entryLinkFactory.CreateEntryLink(entry), 
				HttpUtility.HtmlEncode(contract.Message)), 
				agent);

			new UserCommentNotifier().CheckComment(comment, entryLinkFactory, ctx.OfType<User>());

			return new CommentForApiContract(comment, userIconFactory);

		}

		public void Delete(int commentId) {
			
			var comment = ctx.OfType<T>().Load(commentId);
			var user = ctx.OfType<User>().GetLoggedUser(permissionContext);

			ctx.AuditLogger.AuditLog("deleting " + comment, user);

			if (!user.Equals(comment.Author))
				permissionContext.VerifyPermission(PermissionToken.DeleteComments);

			comment.OnDelete();
			
			ctx.Delete(comment);

		}

		public CommentForApiContract[] GetList(int entryId, int count) {

			return ctx.Query<T>().Where(c => c.EntryForComment.Id == entryId)
				.OrderByDescending(c => c.Created).Take(count).ToArray()
				.Select(c => new CommentForApiContract(c, userIconFactory))
				.ToArray();

        }

		public void Update(int commentId, IComment contract) {
			
			permissionContext.VerifyPermission(PermissionToken.CreateComments);
				
			var comment = ctx.OfType<T>().Load(commentId);

			permissionContext.VerifyAccess(comment, EntryPermissionManager.CanEdit);

			comment.Message = contract.Message;

			ctx.Update(comment);
				
			ctx.AuditLogger.AuditLog("updated " + comment);

		}

	}

}
