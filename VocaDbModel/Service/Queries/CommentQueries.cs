using System.Web;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Queries;

public interface ICommentQueries
{
	CommentForApiContract Create(int entryId, CommentForApiContract contract);

	void Delete(int commentId);

	CommentForApiContract[] GetAll(int entryId);

	int GetCount(int entryId);

	Task<int> GetCountAsync(int entryId);

	CommentForApiContract[] GetList(int entryId, int count);

	Task<CommentForApiContract[]> GetListAsync(int entryId, int count);

	void Update(int commentId, IComment contract);
}

public class CommentQueries<T, TEntry> : ICommentQueries where T : GenericComment<TEntry> where TEntry : class, IEntryWithComments
{
	private readonly IDatabaseContext _ctx;
	private readonly IEntryLinkFactory _entryLinkFactory;
	private readonly Func<int, TEntry>? _entryLoaderFunc;
	private readonly IUserPermissionContext _permissionContext;
	private readonly IUserIconFactory _userIconFactory;

	private TEntry Load(int entryId)
	{
		return _entryLoaderFunc != null ? _entryLoaderFunc(entryId) : _ctx.OfType<TEntry>().Load(entryId);
	}

	public CommentQueries(IDatabaseContext ctx, IUserPermissionContext permissionContext, IUserIconFactory userIconFactory, IEntryLinkFactory entryLinkFactory,
		Func<int, TEntry>? entryLoaderFunc = null)
	{
		_ctx = ctx;
		_entryLinkFactory = entryLinkFactory;
		_permissionContext = permissionContext;
		_userIconFactory = userIconFactory;
		_entryLoaderFunc = entryLoaderFunc;
	}

	public CommentForApiContract Create(int entryId, CommentForApiContract contract)
	{
		ParamIs.NotNull(() => contract);

		_permissionContext.VerifyPermission(PermissionToken.CreateComments);

		if (contract.Author == null || contract.Author.Id != _permissionContext.LoggedUserId)
		{
			throw new NotAllowedException("Can only post as self");
		}

		var entry = Load(entryId);
		var agent = _ctx.OfType<User>().CreateAgentLoginData(_permissionContext, _ctx.OfType<User>().Load(contract.Author.Id));

		var comment = entry.CreateComment(contract.Message, agent);

		_ctx.Save(comment);

		_ctx.AuditLogger.AuditLog($"creating comment for {_entryLinkFactory.CreateEntryLink(entry)}: '{HttpUtility.HtmlEncode(contract.Message)}'",
			agent);

		new UserCommentNotifier().CheckComment(comment, _entryLinkFactory, _ctx.OfType<User>());

		return new CommentForApiContract(comment, _userIconFactory);
	}

	public void Delete(int commentId)
	{
		var comment = _ctx.OfType<T>().Load(commentId);
		var user = _ctx.OfType<User>().GetLoggedUser(_permissionContext);

		_ctx.AuditLogger.AuditLog("deleting " + comment, user);

		if (!user.Equals(comment.Author))
			_permissionContext.VerifyPermission(PermissionToken.DeleteComments);

		comment.Delete();
		_ctx.Update(comment);
	}

	public CommentForApiContract[] GetAll(int entryId)
	{
		return Load(entryId)
			.Comments
			.OrderByDescending(c => c.Created)
			.Select(c => new CommentForApiContract(c, _userIconFactory))
			.ToArray();
	}

	private IQueryable<Comment> GetComments(int entryId) => _ctx.Query<T>()
		.WhereNotDeleted()
		.Where(c => c.EntryForComment.Id == entryId);

	public int GetCount(int entryId) => GetComments(entryId).Count();

	public async Task<int> GetCountAsync(int entryId) => await GetComments(entryId).VdbCountAsync();

	public CommentForApiContract[] GetList(int entryId, int count)
	{
		return GetComments(entryId)
			.OrderByDescending(c => c.Created)
			.Take(count)
			.ToArray()
			.Select(c => new CommentForApiContract(comment: c, iconFactory: _userIconFactory))
			.ToArray();
	}

	public async Task<CommentForApiContract[]> GetListAsync(int entryId, int count)
	{
		var comments = await GetComments(entryId)
			.OrderByDescending(c => c.Created).Take(count)
			.VdbToListAsync();

		return comments
			.Select(c => new CommentForApiContract(c, _userIconFactory))
			.ToArray();
	}

	public void Update(int commentId, IComment contract)
	{
		ParamIs.NotNull(() => contract);

		_permissionContext.VerifyPermission(PermissionToken.CreateComments);

		var comment = _ctx.OfType<T>().Load(commentId);

		_permissionContext.VerifyAccess(comment, EntryPermissionManager.CanEdit);

		comment.Message = contract.Message;

		_ctx.Update(comment);

		_ctx.AuditLogger.AuditLog($"updated comment for {_entryLinkFactory.CreateEntryLink(comment.Entry)}: '{HttpUtility.HtmlEncode(contract.Message)}'");
	}
}
