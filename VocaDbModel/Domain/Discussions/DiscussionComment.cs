using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Discussions {

	public class DiscussionComment : GenericComment<DiscussionTopic> {

		public DiscussionComment() { }

		public DiscussionComment(DiscussionTopic topic, string message, AgentLoginData loginData) 
			: base(topic, message, loginData) {}

		public override void OnDelete() {
			EntryForComment.Comments.Remove(this);
		}

	}

}
