using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Discussions;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// API queries for discussions.
	/// </summary>
	[RoutePrefix("api/discussions")]	
	public class DiscussionApiController : ApiController {

		private const int defaultMax = 10;
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
					.Select(f => new DiscussionFolderContract(f, fields, userIconFactory))
					.ToArray();

			});

		}

		[Route("topics")]
		public PartialFindResult<DiscussionTopicContract> GetTopics(
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
 			DiscussionTopicSortRule sort = DiscussionTopicSortRule.DateCreated,
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) {
			
			return queries.HandleQuery(ctx => {

				var query = ctx.OfType<DiscussionTopic>()
					.Query()
					.Where(f => !f.Deleted);

				var topics = query
					.OrderBy(sort)
					.Paged(new PagingProperties(start, maxResults, getTotalCount))
					.ToArray()
					.Select(f => new DiscussionTopicContract(f, userIconFactory, fields))
					.ToArray();

				var count = (getTotalCount ? query.Count() : 0);

				return PartialFindResult.Create(topics, count);

			});

		}

		[Route("folders/{folderId:int}/topics")]
		public IEnumerable<DiscussionTopicContract> GetTopicsForFolder(int folderId, 
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) {
			
			return queries.HandleQuery(ctx => {
				
				var folder = ctx.Load(folderId);

				return folder.Topics
					.Select(t => new DiscussionTopicContract(t, userIconFactory, fields))
					.OrderByDescending(t => t.LastComment != null ? t.LastComment.Created : t.Created)
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
	
		[Route("comments/{commentId:int}")]
		[Authorize]
		public void PostEditComment(int commentId, CommentForApiContract contract) {
			
			queries.UpdateComment(commentId, contract);

		}

		[Route("topics/{topicId:int}")]
		[Authorize]
		public void PostEditTopic(int topicId, DiscussionTopicContract contract) {
			
			queries.UpdateTopic(topicId, contract);

		}

		[Route("topics/{topicId:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int topicId, CommentForApiContract contract) {
			
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