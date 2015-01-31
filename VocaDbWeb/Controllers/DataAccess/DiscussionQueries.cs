using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Discussions;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Web.Controllers.DataAccess {

	public class DiscussionQueries : QueriesBase<IDiscussionFolderRepository, DiscussionFolder> {

		private readonly IUserIconFactory userIconFactory;

		public DiscussionQueries(IDiscussionFolderRepository repository, IUserPermissionContext permissionContext, IUserIconFactory userIconFactory) 
			: base(repository, permissionContext) {
			
			this.userIconFactory = userIconFactory;

		}

		public CommentForApiContract CreateComment(int topicId, CommentContract contract) {
			
			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			if (contract.Author == null || contract.Author.Id != PermissionContext.LoggedUserId) {
				throw new NotAllowedException("Can only post as self");
			}

			return repository.HandleTransaction(ctx => {
				
				var topic = ctx.OfType<DiscussionTopic>().Load(topicId);
				var agent = ctx.OfType<User>().CreateAgentLoginData(PermissionContext, ctx.OfType<User>().Load(contract.Author.Id));

				var comment = new DiscussionComment(topic, contract.Message, agent);

				ctx.Save(comment);

				ctx.AuditLogger.AuditLog("created " + comment, agent);

				return new CommentForApiContract(comment, userIconFactory);

			});

		}

		public DiscussionFolderContract CreateFolder(DiscussionFolderContract contract) {
			
			// TODO
			PermissionContext.VerifyPermission(PermissionToken.Admin);

			return repository.HandleTransaction(ctx => {
				
				var folder = new DiscussionFolder {
					Name = contract.Name
				};

				ctx.Save(folder);

				ctx.AuditLogger.AuditLog("created " + folder);

				return new DiscussionFolderContract(folder, DiscussionFolderOptionalFields.None);

			});

		}

		public DiscussionTopicContract CreateTopic(int folderId, DiscussionTopicContract contract) {
			
			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			if (contract.Author == null || contract.Author.Id != PermissionContext.LoggedUserId) {
				throw new NotAllowedException("Can only post as self");
			}

			return repository.HandleTransaction(ctx => {
				
				var folder = ctx.Load(folderId);
				var agent = ctx.OfType<User>().CreateAgentLoginData(PermissionContext, ctx.OfType<User>().Load(contract.Author.Id));

				var topic = new DiscussionTopic(folder, contract.Name, contract.Content, agent);

				ctx.Save(topic);

				ctx.AuditLogger.AuditLog("created " + topic, agent);

				return new DiscussionTopicContract(topic, userIconFactory, DiscussionTopicOptionalFields.None);

			});

		}

		public void DeleteComment(int commentId) {
			
			repository.HandleTransaction(ctx => {

				CommentQueries.Create(ctx.OfType<DiscussionComment>(), PermissionContext).Delete(commentId);

			});

		}

		/// <summary>
		/// Soft-deletes a discussion topic.
		/// The topic is marked as deleted, not actually removed from the DB.
		/// User can delete their own topics, moderators can delete all topics.
		/// </summary>
		/// <param name="topicId">Id of the topic to be deleted.</param>
		public void DeleteTopic(int topicId) {
			
			repository.HandleTransaction(ctx => {

				var topic = ctx.OfType<DiscussionTopic>().Load(topicId);
				var user = ctx.OfType<User>().GetLoggedUser(PermissionContext);

				ctx.AuditLogger.AuditLog("deleting " + topic, user);

				if (!user.Equals(topic.Author))
					PermissionContext.VerifyPermission(PermissionToken.DeleteComments);

				topic.Deleted = true;
				ctx.Update(topic);

			});

		}

		public void UpdateComment(int commentId, CommentContract contract) {
			
			repository.HandleTransaction(ctx => {

				CommentQueries.Create(ctx.OfType<DiscussionComment>(), PermissionContext).Update(commentId, contract);
				
			});

		}

		public void UpdateTopic(int topicId, DiscussionTopicContract contract) {
			
			PermissionContext.VerifyPermission(PermissionToken.CreateComments);

			repository.HandleTransaction(ctx => {
				
				var topic = ctx.OfType<DiscussionTopic>().Load(topicId);

				PermissionContext.VerifyAccess(topic, EntryPermissionManager.CanEdit);

				topic.Content = contract.Content;

				ctx.Update(topic);
				ctx.AuditLogger.AuditLog("updated " + topic);

			});

		}

	}

}