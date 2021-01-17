using System;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Queries;

namespace VocaDb.Model.Database.Queries
{
	public class CommentQueries
	{
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
	}
}
