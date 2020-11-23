using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Tags
{
	public class TagComment : GenericComment<Tag>
	{
		public TagComment() { }

		public TagComment(Tag entry, string message, AgentLoginData loginData)
			: base(entry, message, loginData) { }
	}
}
