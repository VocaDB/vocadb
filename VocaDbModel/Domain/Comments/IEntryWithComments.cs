#nullable disable

using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Comments;

public interface IEntryWithComments : IEntryWithNames
{
	IEnumerable<Comment> Comments { get; }

	Comment CreateComment(string message, AgentLoginData loginData);
}

public interface IEntryWithComments<TComment> : IEntryWithComments where TComment : Comment
{
	new IEnumerable<TComment> Comments { get; }
}
