#nullable disable

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtensions;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for entry comments.
	/// </summary>
	[Route("api/comments")]
	[ApiController]
	public class CommentApiController : ApiController
	{
		private readonly CommentQueries _queries;

		public CommentApiController(CommentQueries queries)
		{
			_queries = queries;
		}

		/// <summary>
		/// Deletes a comment.
		/// </summary>
		/// <param name="entryType">Entry type.</param>
		/// <param name="commentId">ID of the comment to be deleted.</param>
		/// <remarks>
		/// Normal users can delete their own comments, moderators can delete all comments.
		/// Requires login.
		/// </remarks>
		[HttpDelete("{entryType}-comments/{commentId:int}")]
		[Authorize]
		public void DeleteComment(EntryType entryType, int commentId) => _queries.DeleteComment(entryType, commentId);

		/// <summary>
		/// Gets a list of comments for an entry.
		/// </summary>
		/// <param name="entryType">Entry type.</param>
		/// <param name="entryId">ID of the entry whose comments to load.</param>
		/// <returns>List of comments in no particular order.</returns>
		[HttpGet("{entryType}-comments")]
		public PartialFindResult<CommentForApiContract> GetComments(EntryType entryType, int entryId) => _queries.GetComments(entryType, entryId);

		/// <summary>
		/// Updates a comment.
		/// </summary>
		/// <param name="entryType">Entry type.</param>
		/// <param name="commentId">ID of the comment to be edited.</param>
		/// <param name="contract">New comment data. Only message can be edited.</param>
		/// <remarks>
		/// Normal users can edit their own comments, moderators can edit all comments.
		/// Requires login.
		/// </remarks>
		[HttpPost("{entryType}-comments/{commentId:int}")]
		[Authorize]
		public void PostEditComment(EntryType entryType, int commentId, CommentForApiContract contract) => _queries.PostEditComment(entryType, commentId, contract);

		/// <summary>
		/// Posts a new comment.
		/// </summary>
		/// <param name="entryType">Entry type.</param>
		/// <param name="contract">Comment data. Message, entry and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[HttpPost("{entryType}-comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(EntryType entryType, CommentForApiContract contract) => _queries.PostNewComment(entryType, contract);

		[HttpGet("")]
		public PartialFindResult<CommentForApiContract> GetList(
			DateTime? before = null,
			DateTime? since = null,
			int? userId = null,
			EntryType entryType = EntryType.Undefined,
			int maxResults = CommentQueries.DefaultMax,
			bool getTotalCount = false,
			CommentOptionalFields fields = CommentOptionalFields.None,
			EntryOptionalFields entryFields = EntryOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default,
			CommentSortRule sortRule = CommentSortRule.CreateDateDescending
		) => _queries.GetList(before, since, userId, entryType, maxResults, getTotalCount, fields, entryFields, lang, sortRule);
	}
}