using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Domain.Users {

	public class EventForUser : IEntryWithIntId {

		private ReleaseEvent releaseEvent;
		private User user;

		public EventForUser() { }

		public EventForUser(User user, ReleaseEvent releaseEvent, UserEventRelationshipType relationshipType) {

			ParamIs.NotNull(() => user);
			ParamIs.NotNull(() => releaseEvent);

			User = user;
			ReleaseEvent = releaseEvent;
			RelationshipType = relationshipType;

		}

		public virtual int Id { get; set; }

		public virtual UserEventRelationshipType RelationshipType { get; set; }

		public virtual ReleaseEvent ReleaseEvent {
			get => releaseEvent;
			set {
				ParamIs.NotNull(() => value);
				releaseEvent = value;
			}
		}

		public virtual User User {
			get => user;
			set {
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

		public virtual void OnDeleted() {
			ReleaseEvent.Users.Remove(this);
			User.Events.Remove(this);
		}

	}

	public enum UserEventRelationshipType {
		Interested,
		Attending
	}

}
