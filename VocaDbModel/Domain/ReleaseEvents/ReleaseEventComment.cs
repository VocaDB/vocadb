using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.ReleaseEvents
{

	public class ReleaseEventComment : GenericComment<ReleaseEvent>
	{

		public ReleaseEventComment() { }
		public ReleaseEventComment(ReleaseEvent entry, string message, AgentLoginData loginData) :
			base(entry, message, loginData)
		{ }

		public override void OnDelete()
		{
			EntryForComment.Comments.Remove(this);
		}

	}

}
