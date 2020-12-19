#nullable disable

using System;
using System.Collections.Generic;
using System.Web.Http;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Discussions;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for discussions.
	/// </summary>
	[RoutePrefix("api/discussions")]
	public class DiscussionApiController : ApiController
	{
		private const int DefaultMax = 10;
		private readonly DiscussionQueries _queries;
		private readonly IUserIconFactory _userIconFactory;

		public DiscussionApiController(DiscussionQueries queries, IUserIconFactory userIconFactory)
		{
			this._queries = queries;
			this._userIconFactory = userIconFactory;
		}

		[Route("comments/{commentId:int}")]
		[Authorize]
		public void DeleteComment(int commentId) => _queries.DeleteComment(commentId);

		[Route("topics/{topicId:int}")]
		[Authorize]
		public void DeleteTopic(int topicId) => _queries.DeleteTopic(topicId);

		[Route("folders")]
		public IEnumerable<DiscussionFolderContract> GetFolders(
			DiscussionFolderOptionalFields fields = DiscussionFolderOptionalFields.None) => _queries.GetFolders(fields);

		[Route("topics")]
		public PartialFindResult<DiscussionTopicContract> GetTopics(
			int? folderId = null,
			int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
 			DiscussionTopicSortRule sort = DiscussionTopicSortRule.DateCreated,
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) => _queries.GetTopics(folderId, start, maxResults, getTotalCount, sort, fields);

		[Route("folders/{folderId:int}/topics")]
		[Obsolete]
		public IEnumerable<DiscussionTopicContract> GetTopicsForFolder(int folderId,
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) => _queries.GetTopicsForFolder(folderId, fields);

		[Route("topics/{topicId:int}")]
		public DiscussionTopicContract GetTopic(int topicId,
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) => _queries.GetTopic(topicId, fields);

		[Route("comments/{commentId:int}")]
		[Authorize]
		public void PostEditComment(int commentId, CommentForApiContract contract) => _queries.UpdateComment(commentId, contract);

		[Route("topics/{topicId:int}")]
		[Authorize]
		public void PostEditTopic(int topicId, DiscussionTopicContract contract) => _queries.UpdateTopic(topicId, contract);

		[Route("topics/{topicId:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int topicId, CommentForApiContract contract) => _queries.CreateComment(topicId, contract);

		[Route("folders")]
		[Authorize]
		public DiscussionFolderContract PostNewFolder(DiscussionFolderContract contract) => _queries.CreateFolder(contract);

		[Route("folders/{folderId:int}/topics")]
		[Authorize]
		public DiscussionTopicContract PostNewTopic(int folderId, DiscussionTopicContract contract) => _queries.CreateTopic(folderId, contract);
	}
}