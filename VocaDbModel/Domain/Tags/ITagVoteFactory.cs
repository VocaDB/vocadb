#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Tags
{
	public interface ITagVoteFactory
	{
		TagVote CreateTagVote(TagUsage usage, User user);
	}
}
