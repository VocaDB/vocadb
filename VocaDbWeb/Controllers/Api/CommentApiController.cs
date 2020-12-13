using System;
using System.Web.Http;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Queries;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for entry comments.
	/// </summary>
	[RoutePrefix("api/comments")]
	public class CommentApiController : ApiController
	{
		private readonly IRepository db;
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IUserPermissionContext userContext;
		private readonly IUserIconFactory userIconFactory;

		private ICommentQueries GetComments(IDatabaseContext ctx, EntryType entryType)
		{
			switch (entryType)
			{
				case EntryType.ReleaseEvent:
					return new CommentQueries<ReleaseEventComment, ReleaseEvent>(ctx, userContext, userIconFactory, entryLinkFactory);
			}

			throw new ArgumentException("Unsupported entry type: " + entryType, nameof(entryType));
		}

		public CommentApiController(IRepository db, IUserPermissionContext userContext, IUserIconFactory userIconFactory, IEntryLinkFactory entryLinkFactory)
		{
			this.db = db;
			this.userContext = userContext;
			this.userIconFactory = userIconFactory;
			this.entryLinkFactory = entryLinkFactory;
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
		[Route("{entryType}-comments/{commentId:int}")]
		[Authorize]
		public void DeleteComment(EntryType entryType, int commentId) => db.HandleTransaction(ctx => GetComments(ctx, entryType).Delete(commentId));

		/// <summary>
		/// Gets a list of comments for an entry.
		/// </summary>
		/// <param name="entryType">Entry type.</param>
		/// <param name="entryId">ID of the entry whose comments to load.</param>
		/// <returns>List of comments in no particular order.</returns>
		[Route("{entryType}-comments")]
		public PartialFindResult<CommentForApiContract> GetComments(EntryType entryType, int entryId) => new PartialFindResult<CommentForApiContract>(db.HandleQuery(ctx => GetComments(ctx, entryType).GetAll(entryId)), 0);

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
		[Route("{entryType}-comments/{commentId:int}")]
		[Authorize]
		public void PostEditComment(EntryType entryType, int commentId, CommentForApiContract contract) => db.HandleTransaction(ctx => GetComments(ctx, entryType).Update(commentId, contract));

		/// <summary>
		/// Posts a new comment.
		/// </summary>
		/// <param name="entryType">Entry type.</param>
		/// <param name="contract">Comment data. Message, entry and author must be specified. Author must match the logged in user.</param>
		/// <returns>Data for the created comment. Includes ID and timestamp.</returns>
		[Route("{entryType}-comments")]
		[Authorize]
		public CommentForApiContract PostNewComment(EntryType entryType, CommentForApiContract contract) => db.HandleTransaction(ctx => GetComments(ctx, entryType).Create(contract.Entry.Id, contract));
	}
}