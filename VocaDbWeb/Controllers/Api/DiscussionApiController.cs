using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Discussions;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.Controllers.Api {

	[RoutePrefix("api/discussions")]	
	public class DiscussionApiController : ApiController {

		private readonly DiscussionQueries queries;
		private readonly IUserIconFactory userIconFactory;

		public DiscussionApiController(DiscussionQueries queries, IUserIconFactory userIconFactory) {
			this.queries = queries;
			this.userIconFactory = userIconFactory;
		}

		[Route("comments/{commentId:int}")]
		public void DeleteComment(int commentId) {
			
			queries.DeleteComment(commentId);

		}

		[Route("topics/{topicId:int}")]
		public void DeleteTopic(int topicId) {
			
			queries.DeleteTopic(topicId);

		}

		[Route("folders")]
		public IEnumerable<DiscussionFolderContract> GetFolders(
			DiscussionFolderOptionalFields fields = DiscussionFolderOptionalFields.None) {
			
			return queries.HandleQuery(ctx => {
				
				return ctx.Query()
					.Where(f => !f.Deleted)
					.OrderBy(f => f.SortIndex)
					.ThenBy(f => f.Name)
					.ToArray()
					.Select(f => new DiscussionFolderContract(f, fields))
					.ToArray();

			});

		}

		[Route("folders/{folderId:int}/topics")]
		public IEnumerable<DiscussionTopicContract> GetTopics(int folderId, 
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) {
			
			return queries.HandleQuery(ctx => {
				
				var folder = ctx.Load(folderId);

				return folder.Topics
					.OrderByDescending(t => t.Created)
					.Select(t => new DiscussionTopicContract(t, userIconFactory, fields))
					.ToArray();

			});

		}
		
		[Route("topics/{topicId:int}")]
		public DiscussionTopicContract GetTopic(int topicId, 
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) {
			
			return queries.HandleQuery(ctx => {
				
				return new DiscussionTopicContract(ctx.OfType<DiscussionTopic>().Load(topicId), userIconFactory, fields);
			
			});

		}
	
		[Route("topics/{topicId:int}")]
		[Authorize]
		public void PostEditTopic(int topicId, DiscussionTopicContract contract) {
			
			queries.UpdateTopic(topicId, contract);

		}

		[Route("topics/{topicId:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int topicId, CommentContract contract) {
			
			return queries.CreateComment(topicId, contract);

		}

		[Route("folders")]
		[Authorize]
		public DiscussionFolderContract PostNewFolder(DiscussionFolderContract contract) {
			
			return queries.CreateFolder(contract);

		}

		[Route("folders/{folderId:int}/topics")]
		[Authorize]
		public DiscussionTopicContract PostNewTopic(int folderId, DiscussionTopicContract contract) {
			
			return queries.CreateTopic(folderId, contract);

		}

	}

}