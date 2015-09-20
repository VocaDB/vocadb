using System.Collections.Generic;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Comments {

	public interface IEntryWithComments : IEntryWithNames {

		IEnumerable<Comment> Comments { get; }

		Comment CreateComment(string message, AgentLoginData loginData);

	}

}
