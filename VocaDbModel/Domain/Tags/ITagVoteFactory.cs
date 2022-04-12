#nullable disable

using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Tags
{
	public interface ITagVoteFactory
	{
		TagVote CreateTagVote(TagUsage usage, User user);
	}
}
