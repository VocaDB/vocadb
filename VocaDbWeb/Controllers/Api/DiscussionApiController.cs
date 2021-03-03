#nullable disable

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Discussions;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Web.Code.Security;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for discussions.
	/// </summary>
	[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
	[Route("api/discussions")]
	[ApiController]
	public class DiscussionApiController : ApiController
	{
		private const int DefaultMax = 10;
		private readonly DiscussionQueries _queries;
		private readonly IUserIconFactory _userIconFactory;

		public DiscussionApiController(DiscussionQueries queries, IUserIconFactory userIconFactory)
		{
			_queries = queries;
			_userIconFactory = userIconFactory;
		}

		[HttpDelete("comments/{commentId:int}")]
		[Authorize]
		public void DeleteComment(int commentId) => _queries.DeleteComment(commentId);

		[HttpDelete("topics/{topicId:int}")]
		[Authorize]
		public void DeleteTopic(int topicId) => _queries.DeleteTopic(topicId);

		[HttpGet("folders")]
		public IEnumerable<DiscussionFolderContract> GetFolders(
			DiscussionFolderOptionalFields fields = DiscussionFolderOptionalFields.None) => _queries.GetFolders(fields);

		[HttpGet("topics")]
		public PartialFindResult<DiscussionTopicContract> GetTopics(
			int? folderId = null,
			int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
 			DiscussionTopicSortRule sort = DiscussionTopicSortRule.DateCreated,
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) => _queries.GetTopics(folderId, start, maxResults, getTotalCount, sort, fields);

		[HttpGet("folders/{folderId:int}/topics")]
		[Obsolete]
		public IEnumerable<DiscussionTopicContract> GetTopicsForFolder(int folderId,
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) => _queries.GetTopicsForFolder(folderId, fields);

		[HttpGet("topics/{topicId:int}")]
		public DiscussionTopicContract GetTopic(int topicId,
			DiscussionTopicOptionalFields fields = DiscussionTopicOptionalFields.None) => _queries.GetTopic(topicId, fields);

		[HttpPost("comments/{commentId:int}")]
		[Authorize]
		public void PostEditComment(int commentId, CommentForApiContract contract) => _queries.UpdateComment(commentId, contract);

		[HttpPost("topics/{topicId:int}")]
		[Authorize]
		public void PostEditTopic(int topicId, DiscussionTopicContract contract) => _queries.UpdateTopic(topicId, contract);

		[HttpPost("topics/{topicId:int}/comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(int topicId, CommentForApiContract contract) => _queries.CreateComment(topicId, contract);

		[HttpPost("folders")]
		[Authorize]
		public DiscussionFolderContract PostNewFolder(DiscussionFolderContract contract) => _queries.CreateFolder(contract);

		[HttpPost("folders/{folderId:int}/topics")]
		[Authorize]
		public DiscussionTopicContract PostNewTopic(int folderId, DiscussionTopicContract contract) => _queries.CreateTopic(folderId, contract);
	}
}