using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Queries {

	public static class CommentQueries {
		public static CommentQueries<T> Create<T>(IRepositoryContext<T> ctx, IUserPermissionContext permissionContext) where T : Comment {
			return new CommentQueries<T>(ctx, permissionContext);
		}
	}

	public class CommentQueries<T> where T : Comment {

		private readonly IRepositoryContext<T> ctx;
		private readonly IUserPermissionContext permissionContext;

		public CommentQueries(IRepositoryContext<T> ctx, IUserPermissionContext permissionContext) {
			this.ctx = ctx;
			this.permissionContext = permissionContext;
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

		public void Update(int commentId, CommentContract contract) {
			
			permissionContext.VerifyPermission(PermissionToken.CreateComments);
				
			var comment = ctx.OfType<DiscussionComment>().Load(commentId);

			permissionContext.VerifyAccess(comment, EntryPermissionManager.CanEdit);

			comment.Message = contract.Message;

			ctx.Update(comment);
				
			ctx.AuditLogger.AuditLog("updated " + comment);

		}

	}

}
