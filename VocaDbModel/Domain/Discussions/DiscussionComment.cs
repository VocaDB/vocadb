namespace VocaDb.Model.Domain.Discussions {

	public class DiscussionComment : Comment {

		private DiscussionTopic topic;

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
