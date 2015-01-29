using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Discussions {

	public class DiscussionComment : Comment {

		private DiscussionTopic topic;

		public DiscussionComment() { }

		public DiscussionComment(DiscussionTopic topic, string message, AgentLoginData loginData) 
			: base(message, loginData) {
			
			Topic = topic;

		}

		public override IEntryWithNames Entry {
			get { return Topic; }
		}

		public virtual DiscussionTopic Topic {
			get { return topic; }
			set {
				ParamIs.NotNull(() => value);
				topic = value;
			}
		}

	}

}
