using System;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Database.Queries
{
	public class CommentQueries
	{
		public const int AbsoluteMax = 500;
		public const int DefaultMax = 50;

		private readonly IRepository _repository;
		private readonly IEntryLinkFactory _entryLinkFactory;
		private readonly IUserPermissionContext _userContext;
		private readonly IUserIconFactory _userIconFactory;

		public CommentQueries(
			IRepository repository,
			IUserPermissionContext userContext,
			IUserIconFactory userIconFactory,
			IEntryLinkFactory entryLinkFactory)
		{
			_repository = repository;
			_userContext = userContext;
			_userIconFactory = userIconFactory;
			_entryLinkFactory = entryLinkFactory;
		}

		private ICommentQueries GetComments(IDatabaseContext ctx, EntryType entryType) => entryType switch
		{
			EntryType.ReleaseEvent => new CommentQueries<ReleaseEventComment, ReleaseEvent>(ctx, _userContext, _userIconFactory, _entryLinkFactory),
			_ => throw new ArgumentException($"Unsupported entry type: {entryType}", nameof(entryType)),
		};

		public void DeleteComment(EntryType entryType, int commentId) => _repository.HandleTransaction(ctx => GetComments(ctx, entryType).Delete(commentId));

		public PartialFindResult<CommentForApiContract> GetComments(EntryType entryType, int entryId) => new(_repository.HandleQuery(ctx => GetComments(ctx, entryType).GetAll(entryId)), 0);

		public void PostEditComment(EntryType entryType, int commentId, CommentForApiContract contract) => _repository.HandleTransaction(ctx => GetComments(ctx, entryType).Update(commentId, contract));

		public CommentForApiContract PostNewComment(EntryType entryType, CommentForApiContract contract) => _repository.HandleTransaction(ctx => GetComments(ctx, entryType).Create(contract.Entry.Id, contract));

		public PartialFindResult<CommentForApiContract> GetList(
			DateTime? before = null,
			DateTime? since = null,
			int? userId = null,
			int maxResults = DefaultMax,
			bool getTotalCount = false,
			CommentSortRule sortRule = CommentSortRule.CreateDateDescending)
		{
			maxResults = Math.Min(maxResults, AbsoluteMax);

			return _repository.HandleQuery(ctx =>
			{
				var query = ctx.Query<Comment>()
					.WhereNotDeleted();

				if (before.HasValue && !since.HasValue)
					query = query.Where(c => c.Created < before.Value);

				if (!before.HasValue && since.HasValue)
					query = query.Where(c => c.Created > since.Value);

				if (before.HasValue && since.HasValue)
					query = query.Where(c => c.Created > since.Value && c.Created < before.Value);

				if (userId.HasValue)
					query = query.Where(a => a.Author.Id == userId.Value);

				var comments = query
					.OrderBy(sortRule)
					.Take(maxResults)
					.ToArray()
					.Select(c => new CommentForApiContract(c, _userIconFactory))
					.ToArray();

				var count = getTotalCount ? query.Count() : 0;

				return PartialFindResult.Create(comments, count);
			});
		}
	}
}
