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

		public virtual bool Equals(DiscussionComment another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as DiscussionComment);
		}

		public override int GetHashCode() {
			return GlobalId.GetHashCode();
		}

		public override void OnDelete() {
			Topic.Comments.Remove(this);
		}

	}

}
