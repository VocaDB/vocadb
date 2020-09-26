using System;
using System.Collections.Generic;
using System.Web.Http;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Discussions;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Service;
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
		[Authorize]
		public void DeleteComment(int commentId) => queries.DeleteComment(commentId);

		[Route("topics/{topicId:int}")]
		[Authorize]
		public void DeleteTopic(int topicId) => queries.DeleteTopic(topicId);

		[Route("folders")]
		public IEnumerable<DiscussionFolderContract> GetFolders(
			DiscussionFolderOptionalFields fields = DiscussionFolderOptionalFields.None) => queries.GetFolders(fields);

		[Route("topics")]
		public PartialFindResult<DiscussionTopicContract> GetTopics(
			int? folderId = null,
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
 			DiscussionTopicSortRule sort = DiscussionTopicSortRule.DateCreated,
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) => queries.GetTopics(folderId, start, maxResults, getTotalCount, sort, fields);

		[Route("folders/{folderId:int}/topics")]
		[Obsolete]
		public IEnumerable<DiscussionTopicContract> GetTopicsForFolder(int folderId,
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) => queries.GetTopicsForFolder(folderId, fields);

		[Route("topics/{topicId:int}")]
		public DiscussionTopicContract GetTopic(int topicId,
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) => queries.GetTopic(topicId, fields);
	
		[Route("comments/{commentId:int}")]
		[Authorize]
		public void PostEditComment(int commentId, CommentForApiContract contract) => queries.UpdateComment(commentId, contract);

		[Route("topics/{topicId:int}")]
		[Authorize]
		public void PostEditTopic(int topicId, DiscussionTopicContract contract) => queries.UpdateTopic(topicId, contract);

		[Route("topics/{topicId:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int topicId, CommentForApiContract contract) => queries.CreateComment(topicId, contract);

		[Route("folders")]
		[Authorize]
		public DiscussionFolderContract PostNewFolder(DiscussionFolderContract contract) => queries.CreateFolder(contract);

		[Route("folders/{folderId:int}/topics")]
		[Authorize]
		public DiscussionTopicContract PostNewTopic(int folderId, DiscussionTopicContract contract) => queries.CreateTopic(folderId, contract);

	}

}